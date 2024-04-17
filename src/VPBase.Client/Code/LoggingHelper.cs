using System;
using Microsoft.Extensions.Logging;
using log4net.Core;

using ILogger = Microsoft.Extensions.Logging.ILogger;
using VPBase.Client.Code.Settings;
using VPBase.Client.Code.Log4Net;
using VPBase.Client.Code.Memory;

namespace VPBase.Logging.Code
{
    public class LoggingHelper
    {
        public const int JobMaxCounterToAvoidSpam = 5;

        public static bool LogJobHeartBeat(string jobName, int counter, string cronSchedulingString, int maxCounterToAvoidSpam = JobMaxCounterToAvoidSpam)
        {
            if (counter == 0 || counter > 0 && counter % maxCounterToAvoidSpam == 0)
            {
                var now = DateTime.UtcNow;

                var extraInfo = $"JobName: {jobName}, " +
                                $"Now (UTC): {now:yyyy-MM-dd HH:mm:ss}, " +
                                $"TIM: {cronSchedulingString}, " +
                                $"C: {counter}, " +
                                $"MC: {maxCounterToAvoidSpam}";

                HeartbeatClient.DefaultClient.Heartbeat(extraInfo, jobName);

                return true;
            }

            return false;
        }

        public static void LogMemoryToLoggerException(ILogger memoryLogger, string memoryLoggerName, ILogger logger, Exception ex)
        {
            memoryLogger.LogError(null, "ExceptionMessage: " + ex.Message);
            var message = MemoryLogger.GetLogGteAsString(memoryLoggerName, LogLevel.Debug);
            logger.LogError(ex, message);
            MemoryLogger.ClearLog(memoryLoggerName);
        }

        public static void LogMemoryToLoggerHighestLevel(string memoryLoggerName, ILogger logger)
        {
            var numOfCriticals = MemoryLogger.GetLogCount(memoryLoggerName, LogLevel.Critical);
            if (numOfCriticals > 0)
            {
                var logs = MemoryLogger.GetLogGteAsString(memoryLoggerName, LogLevel.Debug);
                logger.LogCritical(logs);
                MemoryLogger.ClearLog(memoryLoggerName);
            };

            var numOfErrors = MemoryLogger.GetLogCount(memoryLoggerName, LogLevel.Error);
            if (numOfErrors > 0)
            {
                var logs = MemoryLogger.GetLogGteAsString(memoryLoggerName, LogLevel.Debug);
                logger.LogError(logs);
                MemoryLogger.ClearLog(memoryLoggerName);
            }

            var numOfWarnings = MemoryLogger.GetLogCount(memoryLoggerName, LogLevel.Warning);
            if (numOfWarnings > 0)
            {
                var logs = MemoryLogger.GetLogGteAsString(memoryLoggerName, LogLevel.Debug);
                logger.LogError(logs);
            }

            MemoryLogger.ClearLog(memoryLoggerName);
        }

        public static MonitorSettings GetLoggingSettings()
        {
            // Confirmed to use AppSettingsHelper here since we do not have any constructor here to get
            // AppSettings directly.
            var appSettings = ClientAppSettingsHelper.GetAppSettings();
            return appSettings.MonitorSettings;
        }

        // Filter in programs.cs seems not working, also had problems with filter i log4net.config
        // we skip this here!
        public static bool CheckMessageToBeFiltered(LoggingEvent loggingEvent)
        {
            if (loggingEvent.LoggerName.StartsWith("Hangfire.SqlServer") &&
                (loggingEvent.Level == Level.Info || loggingEvent.Level == Level.Debug))
            {
                return false;
            }

            return true;
        }
    }
}