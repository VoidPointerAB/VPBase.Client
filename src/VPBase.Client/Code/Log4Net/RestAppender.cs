using System;
using System.Collections.Generic;
using System.Threading;
using log4net.Appender;
using log4net.Core;
using log4net.Util;
using VPBase.Auth.Client.Code.ApiClients;
using VPBase.Auth.Client.Models.Logging;
using VPBase.Logging.Code;

namespace VPBase.Client.Code.Log4Net
{
    public class RestAppender : AppenderSkeleton
    {
        private IClientLoggingService _clientLoggingService;

        private ProcessingQueue _processingQueue;

        public string ApplicationName { get; set; }

        public string EndpointUrl { get; set; }

        public int TimeoutSeconds { get; set; }

        public bool DebugMode { get; set; }

        // New 2023-04-21
        private LoggingBaseProperties LoggingBaseProperties { get; set; }

        protected override void OnClose()
        {
            if (_processingQueue != null)
            {
                _processingQueue.WaitUntilIdle(5000);
            }

            base.OnClose();
        }

        public override void ActivateOptions()
        {
            try
            {
                base.ActivateOptions();

                var monitorSettings = LoggingHelper.GetLoggingSettings();

                if (string.IsNullOrEmpty(ApplicationName))
                {
                    ApplicationName = monitorSettings.ApplicationName;
                }

                if (string.IsNullOrEmpty(EndpointUrl))
                {
                    EndpointUrl = monitorSettings.EndpointUrl;
                }

                if (TimeoutSeconds <= 0)
                {
                    TimeoutSeconds = monitorSettings.TimeoutInSeconds;
                }

                DebugMode = monitorSettings.DebugMode;

                _clientLoggingService = ClientLoggingServiceFactory.Create(monitorSettings.UseTestFacade,
                    EndpointUrl,
                    monitorSettings.ClientName,
                    monitorSettings.ClientSecurityKey,
                    monitorSettings.SecondaryEndpointUrl);

                LoggingBaseProperties = ClientLoggingServiceFactory.CreateLoggingBaseProperties(ApplicationName);

                _processingQueue = new ProcessingQueue(LogInternal, 1000, _clientLoggingService, 100);

            }
            catch (Exception ex)
            {
                LogInternal("Unexpected Error in the RestAppender in ActivateOptions", ex);
            }
        }

        /// <summary>
        /// Waits until processing queue has reached the expected queue count.
        /// </summary>
        /// <param name="expectedQueueCount">Expected Queue Count.</param>
        public void WaitUntilQueueIsReached(int expectedQueueCount)
        {
            while (true)
            {
                if (_processingQueue.Count == expectedQueueCount)
                {
                    return;
                }

                Thread.Sleep(500);
            }
        }

        public void WaitUntilQueueIsEmpty(int millisecondsTimeOut = 10000)
        {
            while (true)
            {
                if (_processingQueue.Count == 0)    // Deqeued!
                {
                    _processingQueue.WaitUntilIdle(millisecondsTimeOut);

                    return;
                }

                Thread.Sleep(500);
            }
        }

        private void LogInternal(string message)
        {
            LogInternal(message, null);
        }

        private void LogInternal(string message, Exception ex)
        {
            if (DebugMode)
            {
                LogLog.Debug(typeof(string), message, ex);
            }
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            try
            {
                if (!CheckPreConditions())
                {
                    LogInternal("Preconditions in RestAppender is not set correctly!");
                    return;
                }

                if (!LoggingHelper.CheckMessageToBeFiltered(loggingEvent))
                {
                    return;
                }

                var logValueItem = GetLogValueItem(loggingEvent);

                var exceptionSource = string.Empty;
                if (loggingEvent.ExceptionObject != null)
                {
                    exceptionSource = loggingEvent.ExceptionObject.Source;
                    if (string.IsNullOrEmpty(exceptionSource))
                    {
                        exceptionSource = loggingEvent.ExceptionObject.GetType().ToString();
                    }
                }

                var logMessage = new LogMessage
                {
                    // Base - New 2023-04-21
                    ApplicationName = LoggingBaseProperties.ApplicationName,
                    ApplicationId = LoggingBaseProperties.ApplicationId,
                    FriendlyAppName = LoggingBaseProperties.FriendlyApplicationName,
                    SecretVPMonitorPassword = LoggingBaseProperties.SecretApiKey,
                    TenantIds = LoggingBaseProperties.TenantIds,
                    ApplicationType = LoggingBaseProperties.SystemApplicationType,
                    VersionVPBase = LoggingBaseProperties.VersionVPBase,
                    // Data
                    LogMessageDate = DateTime.Now,
                    LoggerName = loggingEvent.LoggerName ?? string.Empty,
                    ExceptionSource = exceptionSource,
                    ExceptionString = loggingEvent.ExceptionObject != null ? loggingEvent.ExceptionObject.ToString() : string.Empty,
                    CustomerMessage = string.Empty,
                    ExceptionNumber = 0,
                    Level = loggingEvent.Level.Name,
                    Message = GetMessage(loggingEvent),
                    AdditionalInformation = logValueItem.AdditionalInformation ?? string.Empty,
                    User = logValueItem.User,
                    Entity = logValueItem.Entity,
                    Entity2 = logValueItem.Entity2,
                    Url = logValueItem.Url,
                    KeyValues = logValueItem.KeyValues,
                    // New 2023-05-10
                    LogMessageUtcDate = DateTime.UtcNow,
                };

                _processingQueue.Enqueue(logMessage);

                _processingQueue.SendMessagesFromQueue();
            }
            catch (Exception ex)
            {
                LogInternal("RestAppender throwed an unexcepted error!", ex);
            }
        }

        private bool CheckPreConditions()
        {
            if (string.IsNullOrEmpty(ApplicationName))
            {
                return false;
            }

            if (string.IsNullOrEmpty(EndpointUrl))
            {
                return false;
            }

            return true;
        }

        private static string GetMessage(LoggingEvent loggingEvent)
        {
            string message = loggingEvent.RenderedMessage;
            if (string.IsNullOrEmpty(message))
            {
                return string.Empty;
            }

            return message.Trim();
        }

        public LogValueItem GetLogValueItem(LoggingEvent loggingEvent)
        {
            var logValueItem = new LogValueItem();

            var loggingPropertyValues = GetLogPropertyValuesFromEvent(loggingEvent);

            foreach (var loggingPropertyValue in loggingPropertyValues)
            {
                var compareKey = loggingPropertyValue.Key.ToUpper();

                switch (compareKey)
                {
                    case LoggingKeyDefinitions.UserId:
                        {
                            logValueItem.User.UserId = loggingPropertyValue.StringValue;
                            break;
                        }
                    case LoggingKeyDefinitions.UserName:
                        {
                            logValueItem.User.UserName = loggingPropertyValue.StringValue;
                            break;
                        }
                    case LoggingKeyDefinitions.EntityId:
                        {
                            logValueItem.Entity.EntityId = loggingPropertyValue.StringValue;
                            break;
                        }
                    case LoggingKeyDefinitions.EntityValueName:
                        {
                            logValueItem.Entity.EntityValueName = loggingPropertyValue.StringValue;
                            break;
                        }
                    case LoggingKeyDefinitions.EntityTypeName:
                        {
                            logValueItem.Entity.EntityTypeName = loggingPropertyValue.StringValue;
                            break;
                        }
                    case LoggingKeyDefinitions.EntityType:
                        {
                            logValueItem.Entity.EntityType = loggingPropertyValue.IntValue;
                            break;
                        }
                    case LoggingKeyDefinitions.EntityId2:
                        {
                            logValueItem.Entity2.EntityId = loggingPropertyValue.StringValue;
                            break;
                        }
                    case LoggingKeyDefinitions.EntityValueName2:
                        {
                            logValueItem.Entity2.EntityValueName = loggingPropertyValue.StringValue;
                            break;
                        }
                    case LoggingKeyDefinitions.EntityTypeName2:
                        {
                            logValueItem.Entity2.EntityTypeName = loggingPropertyValue.StringValue;
                            break;
                        }
                    case LoggingKeyDefinitions.EntityType2:
                        {
                            logValueItem.Entity2.EntityType = loggingPropertyValue.IntValue;
                            break;
                        }
                    case LoggingKeyDefinitions.RawUrl:
                        {
                            logValueItem.Url.RawUrl = loggingPropertyValue.StringValue;
                            break;
                        }
                    case LoggingKeyDefinitions.AbsoluteUri:
                        {
                            logValueItem.Url.AbsoluteUri = loggingPropertyValue.StringValue;
                            break;
                        }
                    case LoggingKeyDefinitions.AdditionalInformation:
                        {
                            logValueItem.AdditionalInformation = loggingPropertyValue.StringValue;
                            break;
                        }
                    default:
                        {
                            if (!string.IsNullOrEmpty(loggingPropertyValue.DataTypeFullName) &&
                                loggingPropertyValue.ObjectValue != null &&
                                !loggingPropertyValue.Key.ToLower().Contains("log4net"))
                            {
                                var loggingKeyValue = new LoggingKeyValue
                                {
                                    Key = loggingPropertyValue.Key,
                                    Value = loggingPropertyValue.ObjectValue,
                                    DataTypeFullName = loggingPropertyValue.DataTypeFullName
                                };

                                logValueItem.KeyValues.Add(loggingKeyValue);
                            }
                            break;
                        }
                }
            }

            return logValueItem;
        }

        public List<LoggingPropertyValue> GetLogPropertyValuesFromEvent(LoggingEvent loggingEvent)
        {
            var loggingPropertyValues = new List<LoggingPropertyValue>();

            var properties = loggingEvent.GetProperties();

            foreach (var property in properties)
            {
                var objProperty = property;

                if (objProperty is System.Collections.DictionaryEntry)
                {
                    var dictionaryEntry = (System.Collections.DictionaryEntry)objProperty;

                    if (dictionaryEntry.Key is string)
                    {
                        var key = (string)dictionaryEntry.Key;

                        var obj = loggingEvent.LookupProperty(key);

                        var loggingPropertyValue = GetPropertyValueFromKey(key, obj);

                        if (loggingPropertyValue != null && loggingPropertyValue.ObjectValue != null)
                        {
                            loggingPropertyValues.Add(loggingPropertyValue);
                        }
                    }
                }
            }

            return loggingPropertyValues;
        }

        public LoggingPropertyValue GetPropertyValueFromKey(string key, object obj)
        {
            var loggingPropertyValue = new LoggingPropertyValue
            {
                LoggingPropertyType = LoggingPropertyType.Undefined,
                Key = key,
                RawData = obj,
            };

            try
            {
                if (obj != null)
                {
                    loggingPropertyValue.DataTypeFullName = obj.GetType().FullName;

                    if (obj is int || obj is Enum)
                    {
                        loggingPropertyValue.IntValue = (int)obj;
                        loggingPropertyValue.ObjectValue = loggingPropertyValue.IntValue;
                        loggingPropertyValue.DataTypeFullName = typeof(int).FullName;
                        loggingPropertyValue.LoggingPropertyType = LoggingPropertyType.Int;
                        loggingPropertyValue.StringValue = loggingPropertyValue.IntValue.ToString();  // Fallback
                    }
                    else if (obj is string)
                    {
                        loggingPropertyValue.StringValue = (string)obj;
                        loggingPropertyValue.ObjectValue = loggingPropertyValue.StringValue;
                        loggingPropertyValue.DataTypeFullName = typeof(string).FullName;
                        loggingPropertyValue.LoggingPropertyType = LoggingPropertyType.String;
                    }
                    else if (obj is DateTime)
                    {
                        loggingPropertyValue.DateTimeValue = (DateTime)obj;
                        loggingPropertyValue.ObjectValue = loggingPropertyValue.DateTimeValue;
                        loggingPropertyValue.DataTypeFullName = typeof(DateTime).FullName;
                        loggingPropertyValue.LoggingPropertyType = LoggingPropertyType.DateTime;
                    }
                    else if (obj is bool)
                    {
                        loggingPropertyValue.BoolValue = (bool)obj;
                        loggingPropertyValue.ObjectValue = loggingPropertyValue.BoolValue;
                        loggingPropertyValue.DataTypeFullName = typeof(bool).FullName;
                        loggingPropertyValue.LoggingPropertyType = LoggingPropertyType.Bool;
                    }
                    else if (obj is decimal)
                    {
                        loggingPropertyValue.DecimalValue = (decimal)obj;
                        loggingPropertyValue.ObjectValue = loggingPropertyValue.DecimalValue;
                        loggingPropertyValue.DataTypeFullName = typeof(decimal).FullName;
                        loggingPropertyValue.LoggingPropertyType = LoggingPropertyType.Decimal;
                    }
                    else if (obj is LogicalThreadContextStack)
                    {
                        var contextStack = (LogicalThreadContextStack)obj;
                        var contextValue = contextStack.Pop();

                        if (!string.IsNullOrEmpty(contextValue))
                        {
                            var intValue = 0;
                            if (int.TryParse(contextValue, out intValue))
                            {
                                loggingPropertyValue.IntValue = intValue;
                                loggingPropertyValue.ObjectValue = loggingPropertyValue.IntValue;
                                loggingPropertyValue.DataTypeFullName = typeof(int).FullName;
                                loggingPropertyValue.LoggingPropertyType = LoggingPropertyType.Int;
                                loggingPropertyValue.StringValue = loggingPropertyValue.IntValue.ToString();  // Fallback
                            }
                            else
                            {
                                loggingPropertyValue.StringValue = contextValue;
                                loggingPropertyValue.ObjectValue = loggingPropertyValue.StringValue;
                                loggingPropertyValue.DataTypeFullName = typeof(string).FullName;
                                loggingPropertyValue.LoggingPropertyType = LoggingPropertyType.String;
                            }
                        }
                    }
                    else
                    {
                        loggingPropertyValue.ObjectValue = obj;
                        loggingPropertyValue.DataTypeFullName = typeof(object).FullName;
                        loggingPropertyValue.LoggingPropertyType = LoggingPropertyType.Object;
                    }
                }

            }
            catch (Exception ex)
            {
                LogInternal("Error in RestAppender when trying to get and convert key: '" + key +
                    "' with it's data. Wrong datatype!", ex);
            }

            return loggingPropertyValue;
        }
    }
}
