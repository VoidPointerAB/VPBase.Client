using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace VPBase.Client.Code.Memory
{
    public static class MemoryDefaultLogLineFormatter
    {
        /// <summary>
        /// Format string
        /// </summary>
        /// <param name="logLevel">Current line level</param>
        /// <param name="logName">Curent logger name</param>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception (may be null)</param>
        /// <returns>Formatted string</returns>
        public static string Formatter(LogLevel logLevel, string logName, string message, Exception exception)
        {
            var logLevelString = GetRightPaddedLogLevelString(logLevel);

            var taskId = Task.CurrentId;

            //return $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff")} [{taskId}] {logLevelString} - {message}{(exception != null ? $"{Environment.NewLine}{exception}" : "")}";
            return $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff")} [{taskId}] {logLevelString} [{logName}] - {message}{(exception != null ? $"{Environment.NewLine}{exception}" : "")}";
        }
        private static string GetRightPaddedLogLevelString(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return "TRACE";
                case LogLevel.Debug:
                    return "DEBUG";
                case LogLevel.Information:
                    return "INFO ";
                case LogLevel.Warning:
                    return "WARN ";
                case LogLevel.Error:
                    return "ERROR";
                case LogLevel.Critical:
                    return "CRIT ";
                default:
                    return "UNKNO";
            }
        }
    }
}
