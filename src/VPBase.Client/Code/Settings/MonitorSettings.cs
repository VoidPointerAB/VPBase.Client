namespace VPBase.Client.Code.Settings
{
    public class MonitorSettings
    {
        public string ApplicationName { get; set; }

        public string EndpointUrl { get; set; }

        public int TimeoutInSeconds { get; set; }

        public bool DebugMode { get; set; }

        public bool UseTestFacade { get; set; }

        public string ClientName { get; set; }

        public string ClientSecurityKey { get; set; }

        public string SecondaryEndpointUrl { get; set; }

        public string SecretApiKey { get; set; }  // OverrideApiKey
    }
}
