using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using VPBase.Auth.Client.Code.ApiClients;
using VPBase.Auth.Client.Models.Logging;

namespace VPBase.Client.Code.Log4Net
{
    public class ProcessingQueue
    {
        private readonly Action<string, Exception> _logAction;
        private readonly int _maxQueueSize;
        readonly Queue<LogMessage> _queue;
        private readonly object _locker = new object();
        private bool _pending;
        private readonly IClientLoggingService _clientLoggingService;
        private readonly int _maxItemsToSend;
        private readonly ManualResetEvent _pendingEvent = new ManualResetEvent(false);
        private Action<int> _sendFinishedAction;

        private class QueueInfo
        {
            public QueueInfo(Action logAction)
            {
                LogAction = logAction;
            }

            public Action LogAction { get; private set; }
        }

        public ProcessingQueue(Action<string, Exception> logAction, int maxQueueSize, IClientLoggingService clientLoggingService, int maxItemsToSend)
        {
            _logAction = logAction;
            _maxQueueSize = maxQueueSize;
            _clientLoggingService = clientLoggingService;
            _maxItemsToSend = maxItemsToSend;
            _queue = new Queue<LogMessage>();
        }

        public int Count
        {
            get
            {
                lock (_locker)
                {
                    return _queue.Count;
                }
            }
        }

        public void AddSendFinishedAction(Action<int> action)
        {
            _sendFinishedAction = action;
        }

        public void Enqueue(LogMessage logMessage)
        {
            lock (_locker)
            {
                if (_queue.Count >= _maxQueueSize)
                {
                    _queue.Dequeue();
                    _logAction(string.Format("Queue contains more then {0} items. Dequeue one item before Enqueue!", _maxQueueSize), null);
                }

                _queue.Enqueue(logMessage);
            }
        }

        public void SendMessagesFromQueue()
        {
            try
            {
                lock (_locker)
                {
                    if (_pending == false)
                    {
                        _pendingEvent.Reset();
                        _pending = true;
                        ThreadPool.QueueUserWorkItem(DoWork);
                    }
                }
            }
            catch (Exception ex)
            {
                _logAction(string.Format("SendMessagesFromQueue throwed exception!"), ex);
            }
        }

        public LogMessageList DequeueList(int numOfItems)
        {
            lock (_locker)
            {
                var logMessageList = new LogMessageList();
                if (logMessageList.LogMessages == null)
                {
                    logMessageList.LogMessages = new List<LogMessage>();
                }

                var numOfDequeues = numOfItems;
                if (_queue.Count < numOfItems)
                {
                    numOfDequeues = _queue.Count;
                }

                for (var i = 0; i < numOfDequeues; ++i)
                {
                    var logMessage = _queue.Dequeue();
                    logMessageList.LogMessages.Add(logMessage);
                }

                return logMessageList;
            }
        }

        public void DoWork(object state)
        {
            var messagesSent = 0;
            var sleepInMsCalc = ProcessingQueueDefinitions.SleepInMs;
            var nameOfProcess = "MessageQueue";

            while (true)
            {
                try
                {

                    var messageList = DequeueList(_maxItemsToSend);

                    if (messageList.LogMessages.Any())
                    {
                        var sendResult = false;

                        try
                        {
                            var restResult = _clientLoggingService.SendLogMessages(messageList);
                            sendResult = restResult;
                            messagesSent += messageList.LogMessages.Count;
                        }
                        catch (Exception ex)
                        {
                            var errorMessage = ex.Message;
                            if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
                            {
                                errorMessage += ", " + ex.InnerException.Message;
                            }
                            sendResult = false;
                            _logAction(nameOfProcess + " throwed an exception! Message: " + errorMessage, null);
                        }

                        // It failed, put the messages in the queue again and try send it next loop
                        if (!sendResult)
                        {
                            foreach (var logMessage in messageList.LogMessages)
                            {
                                Enqueue(logMessage);
                            }

                            sleepInMsCalc = (int)((sleepInMsCalc) * ProcessingQueueDefinitions.SleepFactor);   // Increase sleep time since failure!
                        }
                        else
                        {
                            sleepInMsCalc = ProcessingQueueDefinitions.SleepInMs;
                        }
                    }

                    var timeIsUp = (sleepInMsCalc > ProcessingQueueDefinitions.SleepCalcTimesUpInMs);

                    if (Count == 0 || timeIsUp)    // Jump out when no more in queue or time is up!
                    {
                        if (timeIsUp)
                        {
                            _logAction(nameOfProcess + " timesUp reached!", null);
                        }

                        if (Count == 0)
                        {
                            _logAction(nameOfProcess + " no more items in queue!", null);
                        }

                        lock (_locker)
                        {
                            _pending = false;
                            _pendingEvent.Set();
                            break;
                        }
                    }

                    Thread.Sleep(sleepInMsCalc);
                    _logAction(nameOfProcess + " waiting: " + (sleepInMsCalc / 1000) + " s", null);
                }
                catch (Exception ex)
                {
                    _logAction(nameOfProcess + " DoWork throwed excpetion!", ex);
                }
            }

            if (_sendFinishedAction != null)
            {
                _logAction(nameOfProcess + " finished!", null);
                _sendFinishedAction.Invoke(messagesSent);
            }
        }

        public void WaitUntilIdle(int millisecondsTimeout)
        {
            try
            {
                if (_pendingEvent.WaitOne(millisecondsTimeout))
                {
                }
            }
            catch (Exception)
            {
            }
        }
    }

    public class ProcessingQueueDefinitions
    {
        public const int SleepInMs = 1000;
        public const double SleepFactor = 1.5;
        public const int SleepCalcTimesUpInMs = 60000; // 1 min factor reached (ex: 1000 * 1,5 * 1,5 etc), approx 5 min
    }
}
