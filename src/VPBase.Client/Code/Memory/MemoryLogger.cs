using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace VPBase.Client.Code.Memory
{
    public class MemoryLogDictionaryItem
    {
        public LogLevel LogLevel { get; set; }

        public MemoryLoggerItem Item { get; set; }
    }

    public class MemoryLogger : ILogger
    {
        private static readonly object LockObj = new object();

        private Func<string, LogLevel, bool> _filter;

        private readonly Func<LogLevel, string, string, Exception, string> _logLineFormatter = null;

        // VP: removed!
        //private static readonly Dictionary<LogLevel, MemoryLoggerItem> LogsDictionary = new Dictionary<LogLevel, MemoryLoggerItem>();

        // VP: changed to string to be key instead for level. The string key contains the name and level
        private static readonly Dictionary<string, MemoryLogDictionaryItem> LogsDictionary = new Dictionary<string, MemoryLogDictionaryItem>();

        public static int MaxLogCount = 200;

        public string Name { get; }

        public Func<string, LogLevel, bool> Filter
        {
            get { return _filter; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _filter = value;
            }
        }

        /// <summary>
        /// Last <see cref="MaxLogCount"/> log lines
        /// </summary>
        public static List<string> LogList
        {
            get
            {
                return LogsDictionary
                            .SelectMany(x => x.Value.Item.logList)
                            .OrderByDescending(x => x.Item1)
                            .Take(MaxLogCount)
                            .Select(x => x.Item2)
                            .Reverse() // keep asc sort like in 1st version
                            .ToList();
            }
        }

        /// <summary>
        /// Return last <see cref="MaxLogCount"/> log lines with specified logLevel
        /// </summary>
        /// <param name="logLevel">log level for return</param>
        /// <returns></returns>
        public static List<string> GetLog(string name, LogLevel logLevel)
        {
            // VP: changed to string to be key instead for level 
            var logKey = CreateDictionaryKey(name, logLevel);

            if (LogsDictionary.TryGetValue(logKey, out var log))
            {
                return log.Item.logList.OrderBy(x => x.Item1).Select(x => x.Item2).ToList();
            }
            else
            {
                return Enumerable.Empty<string>().ToList();
            }
        }

        // VP-method
        public static int GetLogCount(string name, LogLevel logLevel)
        {
            // VP: changed to string to be key instead for level 
            var prefixKey = GetDictionaryPrefixKey(name);

            return LogsDictionary.Where(x => x.Key.Contains(prefixKey) &&
                                             x.Value.LogLevel == logLevel).SelectMany(x => x.Value.Item.logList).Count();
        }

        // VP-method
        public static int GetLogGteCount(string name, LogLevel minLogLevel)
        {
            // VP: changed to string to be key instead for level 
            var prefixKey = GetDictionaryPrefixKey(name);

            return LogsDictionary.Where(x => x.Key.Contains(prefixKey) &&
                                             x.Value.LogLevel >= minLogLevel).SelectMany(x => x.Value.Item.logList).Count();
        }

        /// <summary>
        /// Return log lines with logLevel more or equal than <paramref name="minLogLevel"/>
        /// </summary>
        /// <param name="minLogLevel">Min log level</param>
        /// <returns>List of log lines</returns>
        public static List<string> GetLogGte(string name, LogLevel minLogLevel)
        {
            // VP: changed to string to be key instead for level 
            var prefixKey = GetDictionaryPrefixKey(name);

            return LogsDictionary.Where(x => x.Key.Contains(prefixKey) &&
                                        x.Value.LogLevel >= minLogLevel)
                                        .SelectMany(x => x.Value.Item.logList).OrderBy(x => x.Item1).Select(x => x.Item2).ToList();
        }

        // VP-method
        public static string GetLogGteAsString(string name, LogLevel minLogLevel)
        {
            var logs = GetLogGte(name, minLogLevel);

            var message = Environment.NewLine + "{" + Environment.NewLine;

            message += "\tHistory: " + Environment.NewLine;

            foreach (var logItem in logs)
            {
                message += "\t" + logItem + Environment.NewLine;
            }

            return message += "}" + Environment.NewLine;
        }

        // VP-method
        public static void ClearLog(string name)
        {
            var prefixKey = GetDictionaryPrefixKey(name);

            var listOfKeys = LogsDictionary.Where(x => x.Key.Contains(prefixKey)).Select(x => x.Key).ToList();

            lock (LockObj)
            {
                foreach (var key in listOfKeys)
                {
                    LogsDictionary.Remove(key);
                }
            }
        }

        /// <summary>
        /// Return log lines with logLevel less or equal than <paramref name="maxLogLevel"/>
        /// </summary>
        /// <param name="maxLogLevel">Max log level</param>
        /// <returns>List of log lines</returns>
        public static List<string> GetLogLte(string name, LogLevel maxLogLevel)
        {
            // VP: changed to string to be key instead for level 
            var prefixKey = GetDictionaryPrefixKey(name);

            return LogsDictionary.Where(x => x.Key.Contains(prefixKey) &&
                                        x.Value.LogLevel <= maxLogLevel)
                                        .SelectMany(x => x.Value.Item.logList)
                                        .OrderBy(x => x.Item1).Select(x => x.Item2).ToList();
        }

        // VP-method
        public static string GetDictionaryPrefixKey(string name)
        {
            return name + "_";
        }

        // VP-method
        public static string CreateDictionaryKey(string name, LogLevel logLevel)
        {
            return name + "_" + logLevel;
        }

        // VP removed. Not needed!
        //static MemoryLogger()
        //{
        //    foreach (var level in ((LogLevel[])Enum.GetValues(typeof(LogLevel))).Where(x => x != LogLevel.None))
        //    {
        //        var key = CreateDictionaryKey("_general", level);
        //        LogsDictionary.Add(key, new MemoryLogDictionaryItem()
        //        {
        //            LogLevel = level,
        //            Item = new MemoryLoggerItem(MaxLogCount)
        //        }); 
        //    }
        //}

        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="name">logger name</param>
        /// <param name="filter">filter log entities</param>
        /// <param name="maxLogCount">max count of stored lines of log (for each level)</param>
        /// <param name="logLineFormatter">string formatter for log line</param>
        public MemoryLogger(string name, Func<string, LogLevel, bool> filter,
            int maxLogCount,
            Func<LogLevel, string, string, Exception, string> logLineFormatter)
        {
            Name = name;
            _filter = filter ?? ((category, logLevel) => true);
            MaxLogCount = maxLogCount;
            _logLineFormatter = logLineFormatter ?? MemoryDefaultLogLineFormatter.Formatter;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            // VP: Added code below, we create the key instead when we need it! Instead of using the static constuctor
            var logKey = CreateDictionaryKey(Name, logLevel);
            if (!LogsDictionary.TryGetValue(logKey, out var test))
            {
                LogsDictionary.Add(logKey, new MemoryLogDictionaryItem() { Item = new MemoryLoggerItem(MaxLogCount), LogLevel = logLevel });
            }

            if (LogsDictionary.TryGetValue(logKey, out var currentLog))
            {
                if (!string.IsNullOrEmpty(message))
                {
                    var preparedMessage = _logLineFormatter(logLevel, Name, message, exception);
                    lock (LockObj)
                    {
                        if (currentLog.Item.logList.Count < MaxLogCount)
                        {
                            currentLog.Item.logList.Add(new Tuple<DateTime, string>(DateTime.Now, preparedMessage));
                        }
                        else
                        {
                            currentLog.Item.logList[currentLog.Item.currentLogIndex] = new Tuple<DateTime, string>(DateTime.Now, preparedMessage);
                        }

                        if (currentLog.Item.currentLogIndex < MaxLogCount - 1)
                        {
                            currentLog.Item.currentLogIndex++;
                        }
                        else
                        {
                            currentLog.Item.currentLogIndex = 0;
                        }
                    }

                }
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _filter(Name, logLevel);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            return MemoryLogScope.Push(Name, state);
        }
    }
}