using System;
using System.Threading;
using VPBase.Auth.Client.Code.ApiClients;
using VPBase.Auth.Client.Models.Logging;
using VPBase.Client.Code.Settings;
using VPBase.Logging.Code;

namespace VPBase.Client.Code.Log4Net
{
    public class HeartbeatClient : IHeartbeatClient
    {
        private IClientLoggingService _clientLoggingService;

        // New 2023-04-21
        private LoggingBaseProperties LoggingBaseProperties { get; set; }

        public HeartbeatClient(MonitorSettings loggingSettings)
        {
            _clientLoggingService = ClientLoggingServiceFactory.Create(loggingSettings.UseTestFacade,
                loggingSettings.EndpointUrl,
                loggingSettings.ClientName,
                loggingSettings.ClientSecurityKey,
                loggingSettings.SecondaryEndpointUrl);

            LoggingBaseProperties = ClientLoggingServiceFactory.CreateLoggingBaseProperties(loggingSettings.ApplicationName);
        }

        public void Heartbeat()
        {
            Heartbeat(null);
        }

        public void Heartbeat(string additionalInfo, string identifier = null, int? intervalInSeconds = null)
        {
            try
            {
                ThreadPool.QueueUserWorkItem(o =>
                {
                    try
                    {
                        _clientLoggingService.SendHeartbeatMessage(new HeartbeatMessage
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
                            AdditionalInformation = additionalInfo,
                            // New 2023-05-10
                            HearbeatMessageDate = DateTime.Now,
                            HearbeatMessageUtcDate = DateTime.UtcNow,
                            Identifier = identifier,
                            IntervalInSeconds = intervalInSeconds
                        });

                        Thread.Sleep(750);
                    }
                    catch (Exception ex)
                    {
                        var test = ex.Message;
                    }
                    finally
                    {
                        Thread.Sleep(750);
                    }
                });
            }
            catch (Exception ex)
            {
                var test = ex.Message;
            }
        }

        private static IHeartbeatClient _defaultClient;

        private static readonly object DefaultClientLock = new object();

        public static IHeartbeatClient DefaultClient
        {
            get
            {
                if (_defaultClient == null)
                {
                    lock (DefaultClientLock)
                    {
                        if (_defaultClient == null)
                        {
                            var monitorSettings = LoggingHelper.GetLoggingSettings();

                            _defaultClient = new HeartbeatClient(monitorSettings);
                        }
                    }
                }

                return _defaultClient;
            }
        }
    }
}
