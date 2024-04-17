using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace VPBase.Client.Code.Memory.Settings
{
    public class MemoryLoggerSettings : IMemoryLoggerSettings
    {

        public IDictionary<string, LogLevel> Switches { get; set; }
            = new Dictionary<string, LogLevel>();

        /// <summary>
        /// Include scope flag
        /// </summary>
        public bool IncludeScopes { get; set; }

        #region IMemoryLoggerSettings implementation

        /// <inheritdoc />
        public int MaxLogCount { get; set; }

        /// <inheritdoc />
        public IChangeToken ChangeToken { get; set; }

        /// <inheritdoc />
        public IMemoryLoggerSettings Reload()
        {
            return this;
        }

        /// <inheritdoc />
        public bool TryGetSwitch(string name, out LogLevel level)
        {
            return Switches.TryGetValue(name, out level);
        }

        #endregion
    }
}
