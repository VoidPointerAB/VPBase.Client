using log4net.Core;
using NUnit.Framework;
using VPBase.Auth.Client.Models.Logging;
using VPBase.Client.Code.Log4Net;
using VPBase.Client.Code.Settings;

namespace VPBase.Client.Tests.Logging
{
    [TestFixture]
    public class RestAppenderExplicitTests
    {
        [SetUp]     
        public void Setup()
        {
            var clientAppSettings = new ClientAppSettings
            {
                FriendlyApplicationName = "VPBaseClientTests",
                ApplicationId = "TEST_Base_Client_TestApp",
                TenantIds = "TEST_VPBase_Tenant_TestCompany",

                MonitorSettings = new MonitorSettings()     // Logging
                {
                    ApplicationName = "VPBaseClientApplicationName",
                    EndpointUrl = "https://xxxxx.net",

                    SecretApiKey = "key", 
                }
            };

            ClientAppSettingsHelper.ApplyAppSettings(clientAppSettings);
        }


        [Test, Explicit]
        public void Explicit_where_send_info_message()
        {
            var restAppender = new RestAppender();

            restAppender.ActivateOptions();  // This must be runned first to activate the appender!

            var dateStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            var logEventData = new LoggingEventData
            {
                LoggerName = "TestLoggerName",
                Level = Level.Info,
                Message = $"Hello! My local time is: {dateStr}" ,
                TimeStampUtc = DateTime.UtcNow,
                ThreadName = "Test",
            };

            logEventData.Properties = new log4net.Util.PropertiesDictionary();

            logEventData.Properties[LoggingKeyDefinitions.EntityId] = "3456";
            logEventData.Properties[LoggingKeyDefinitions.EntityValueName] = "4567";
            logEventData.Properties[LoggingKeyDefinitions.EntityTypeName] = "5678";
            logEventData.Properties[LoggingKeyDefinitions.EntityType] = 123;

            Task.Run(()=> { SendMessage(restAppender, logEventData); }).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private void SendMessage(RestAppender restAppender, LoggingEventData loggingEventData)
        {
            try
            {
                restAppender.DoAppend(new LoggingEvent(loggingEventData));  // Append log message to queue

                restAppender.WaitUntilQueueIsReached(1);

                Console.WriteLine($"Message successfully added!");

                restAppender.WaitUntilQueueIsEmpty();

                Console.WriteLine($"Message: '{loggingEventData.Message}' successfully sent!");
            }
            catch(Exception ex) 
            {
                Console.WriteLine($"Message: '{loggingEventData.Message}' failure! Exception message: {ex.Message}");

                Assert.Fail(ex.Message);
            }
        }
    }
}
