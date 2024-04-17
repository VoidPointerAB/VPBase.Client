using System;
using System.Collections.Generic;

namespace VPBase.Client.Code.Settings
{
    public class ClientAppSettings
    {
        public ClientAppSettings()
        {
            MonitorSettings = new MonitorSettings();
        }

        public MonitorSettings MonitorSettings { get; set; }

        public string ApplicationId { get; set; }

        public string FriendlyApplicationName { get; set; }

        public string TenantIds { get; set; }

        public List<string> TenantIdList
        {
            get
            {
                if (!string.IsNullOrEmpty(TenantIds))
                {
                    var tenantIds = TenantIds.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (tenantIds.Length > 0)
                    {
                        return new List<string>(tenantIds);
                    }
                }
                return new List<string>();
            }
        }
    }
}
