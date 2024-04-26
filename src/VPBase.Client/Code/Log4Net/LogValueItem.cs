using System.Collections.Generic;
using VPBase.Auth.Client.Models.Logging;

namespace VPBase.Client.Code.Log4Net
{
    public class LogValueItem
    {
        public LogValueItem()
        {
            User = new LoggingUser();
            Url = new LoggingUrl();
            Entity = new LoggingEntity();
            Entity2 = new LoggingEntity();
            KeyValues = new List<LoggingKeyValue>();
        }

        public LoggingUser User { get; set; }

        public LoggingEntity Entity { get; set; }

        public LoggingEntity Entity2 { get; set; }

        public LoggingUrl Url { get; set; }

        public List<LoggingKeyValue> KeyValues { get; set; }

        public string AdditionalInformation { get; set; }
    }
}
