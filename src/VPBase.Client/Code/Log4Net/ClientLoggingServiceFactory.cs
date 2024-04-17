using VPBase.Auth.Client.Code.ApiClients;
using VPBase.Auth.Client.Code.ApiClients.Connections;
using VPBase.Auth.Client.Code.ApiClients.InMemory;
using VPBase.Auth.Client.Code.ApiClients.Rest;
using VPBase.Auth.Client.Types;
using System.Collections.Generic;
using VPBase.Auth.Client.Models.ReportInfos;
using Microsoft.Extensions.Logging;
using VPBase.Client.Code.Shared.AuthContract;
using VPBase.Client.Code.Settings;
using VPBase.Client.Code.Shared;

namespace VPBase.Client.Code.Log4Net
{
    public class ClientLoggingServiceFactory
    {
        public static IClientLoggingService Create(bool useTestFacade, string endPointUrl, string clientName, string clientSecurityKey, string secondaryEndpointUrl)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

            if (!useTestFacade && !string.IsNullOrEmpty(endPointUrl))
            {
                if (string.IsNullOrEmpty(clientName))
                {
                    clientName = string.Empty;
                }
                if (string.IsNullOrEmpty(clientSecurityKey))
                {
                    clientSecurityKey = string.Empty;
                }
                if (string.IsNullOrEmpty(secondaryEndpointUrl))
                {
                    secondaryEndpointUrl = string.Empty;
                }

                var loggingSettingsConnection = new LoggingSettingsConnection
                {
                    PrimaryServerUrl = endPointUrl,
                    ClientName = clientName,
                    ClientSecurityKey = clientSecurityKey,
                    SecondaryServerUrl = secondaryEndpointUrl
                };

                var authJsonConverter = new AuthContractJsonConverter();
                var loggerFactory = new LoggerFactory();
                var authContractLogger = new AuthContractLogger(loggerFactory);
                var authClientHelper = new AuthClientHelper(authJsonConverter);
                var authApiHelper = new AuthApiHelper(authJsonConverter);

                return new RestClientLoggingService(loggingSettingsConnection, authContractLogger, authClientHelper, authApiHelper);
            }
            else
            {
                return new InMemoryClientLoggingService();
            }
        }

        public static LoggingBaseProperties CreateLoggingBaseProperties(string applicationName)
        {
            var appSettings = ClientAppSettingsHelper.GetAppSettings();

            var loggingProperties = new LoggingBaseProperties
            {
                ApplicationName = applicationName,
                ApplicationId = appSettings.ApplicationId,
                FriendlyApplicationName = appSettings.FriendlyApplicationName,
                TenantIds = appSettings.TenantIdList,

                SystemApplicationType = SystemApplicationType.Web,
                VersionVPBase = (int)VPBaseVersion.Version_4,

                SecretApiKey = appSettings.MonitorSettings.SecretApiKey,
            };

            return loggingProperties;
        }
    }

    public class LoggingBaseProperties
    {
        public string ApplicationName { get; set; }

        public string ApplicationId { get; set; }

        public string FriendlyApplicationName { get; set; }

        public string SecretApiKey { get; set; }

        public List<string> TenantIds { get; set; }

        public SystemApplicationType SystemApplicationType { get; set; }

        public int VersionVPBase { get; set; }
    }
}
