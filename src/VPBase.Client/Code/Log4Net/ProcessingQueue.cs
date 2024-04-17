using System;
using System.Collections.Generic;
using System.Threading;

namespace VPBase.Client.Code.Log4Net
{
    public class ProcessingQueue
    {
        private readonly Action<string, Exception> _logAction;
        private readonly int _maxQueueSize;
        readonly Queue<QueueInfo> _queue;
        private readonly object _queueSync;
        private bool _pending;
        private readonly ManualResetEvent _pendingEvent = new ManualResetEvent(false);

        private class QueueInfo
        {
            public QueueInfo(Action logAction)
            {
                LogAction = logAction;
            }

            public Action LogAction { get; private set; }
        }

        public ProcessingQueue(Action<string, Exception> logAction, int maxQueueSize)
        {
            _logAction = logAction;
            _maxQueueSize = maxQueueSize;
            _queueSync = new object();
            _queue = new Queue<QueueInfo>();
        }

        public void Enqueue(Action action)
        {
            lock (_queueSync)
            {
                if (_queue.Count > _maxQueueSize)
                {
                    _queue.Dequeue();
                    _logAction(string.Format("Queue contains more then {0} items. Dequeue one item before Enqueue!", _maxQueueSize), null);
                }

                _queue.Enqueue(new QueueInfo(action));

                if (_pending == false)
                {
                    _pendingEvent.Reset();
                    _pending = true;
                    ThreadPool.QueueUserWorkItem(DoWork);
                }
            }
        }

        public void DoWork(object state)
        {
            while (true)
            {
                try
                {
                    QueueInfo queueInfo = null;

                    lock (_queueSync)
                    {
                        if (_queue.Count == 0)
                        {
                            _pending = false;
                            _pendingEvent.Set();
                            break;
                        }

                        queueInfo = _queue.Dequeue();
                    }

                    if (queueInfo != null && queueInfo.LogAction != null)
                    {
                        queueInfo.LogAction();
                    }

                    Thread.Sleep(0);
                }
                catch (Exception ex)
                {
                    _logAction(string.Format("DoWork throwed excpetion!"), ex);
                }
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
}
