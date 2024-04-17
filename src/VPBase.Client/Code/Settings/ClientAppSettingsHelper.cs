//using Microsoft.Extensions.Configuration;

namespace VPBase.Client.Code.Settings
{
    public class ClientAppSettingsHelper
    {
        private static ClientAppSettings _clientSettings = null;

        public static ClientAppSettings GetAppSettings()
        {
            return _clientSettings;
        }

        #region Testing 

        public static void Clear()
        {
            ApplyAppSettings(null);
        }

        public static void ApplyAppSettings(ClientAppSettings clientSettings)
        {
            _clientSettings = clientSettings;
        }

        #endregion
    }
}
