using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VPBase.Auth.Contract.SharedInterfaces;

namespace VPBase.Client.Code.Shared.AuthContract
{
    public class AuthContractLogger : IAuthContractLogger
    {
        private ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private string _loggerName;

        public AuthContractLogger(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            SetLoggerName("NetCoreAuthContractLogger");

        }

        public void LogDebug(string message)
        {
            _logger.LogDebug(message);
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        public void LogWarning(string message)
        {
            _logger.LogWarning(message);
        }

        public void LogWarning(string message, Exception ex)
        {
            _logger.LogWarning(ex, message);
        }

        public void LogError(string message)
        {
            _logger.LogError(message);
        }

        public void LogError(string message, Exception ex)
        {
            _logger.LogError(ex, message);
        }

        #region Name

        public void SetLoggerName(string name)
        {
            _logger = _loggerFactory.CreateLogger(name);
            _loggerName = name;
        }

        public void SetLoggerNameByType(Type type)
        {
            _logger = _loggerFactory.CreateLogger(type);
            _loggerName = type.Name;
        }

        public string GetLoggerName()
        {
            return _loggerName;
        }

        #endregion
    }

    public class TestAuthContractLogger : IAuthContractLogger
    {
        private string _loggerName = MethodBase.GetCurrentMethod().DeclaringType.Name;

        private bool _enableLogging = false;

        private List<AuthContractLogItem> _logItems = new List<AuthContractLogItem>();

        private int _logCounter = 0;

        #region Logging

        public void LogDebug(string message)
        {
            Log(AuthContractLogItemType.Debug, message);
        }

        public void LogInformation(string message)
        {
            Log(AuthContractLogItemType.Info, message);
        }

        public void LogWarning(string message)
        {
            Log(AuthContractLogItemType.Warning, message);
        }

        public void LogWarning(string message, Exception ex)
        {
            Log(AuthContractLogItemType.Warning, message, ex);
        }

        public void LogError(string message)
        {
            Log(AuthContractLogItemType.Error, message);
        }

        public void LogError(string message, Exception ex)
        {
            Log(AuthContractLogItemType.Error, message, ex);
        }

        #endregion

        #region Name

        public void SetLoggerName(string name)
        {
            _loggerName = name;
        }

        public void SetLoggerNameByType(Type type)
        {
            _loggerName = type.FullName;
        }

        public string GetLoggerName()
        {
            return _loggerName;
        }

        #endregion

        public void EnableLogging(bool enable)
        {
            _enableLogging = enable;
        }

        public IEnumerable<AuthContractLogItem> GetLogs(AuthContractLogItemType logItemType)
        {
            if (logItemType == AuthContractLogItemType.All)
            {
                return _logItems;
            }
            else
            {
                return _logItems.Where(x => x.LogItemType == logItemType).ToList();
            }
        }

        private void Log(AuthContractLogItemType logItemType, string message, Exception exception = null)
        {
            if (_enableLogging)
            {
                _logCounter++;

                var logItem = new AuthContractLogItem
                {
                    Id = _logCounter,
                    Message = message,
                    LogItemType = logItemType,
                    DateTimeLocal = DateTime.Now,
                    DateTimeUtc = DateTime.UtcNow,
                    Exception = exception
                };

                _logItems.Add(logItem);
            }
        }
    }

    public class AuthContractLogItem
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public Exception Exception { get; set; }

        public DateTime DateTimeLocal { get; set; }

        public DateTime DateTimeUtc { get; set; }

        public AuthContractLogItemType LogItemType { get; set; }
    }

    public enum AuthContractLogItemType
    {
        Debug = 0,
        Info = 1,
        Warning = 2,
        Error = 3,
        All = 10
    }
}
