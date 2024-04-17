using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;

namespace VPBase.Client.Code.Memory.Settings
{
    // <summary>
    ///  IConfiguration IMemoryLoggerSettings realization 
    /// </summary>
    public class MemoryConfigurationLoggerSettings : IMemoryLoggerSettings
    {
        private const int DefaultMaxLogCount = 200;

        private readonly IConfiguration _configuration;

        /// <summary>
        /// constractor
        /// </summary>
        public MemoryConfigurationLoggerSettings(IConfiguration configuration)
        {
            _configuration = configuration;
            ChangeToken = configuration.GetReloadToken();
        }

        #region IMemoryLoggerSettings implementation

        /// <inheritdoc />
        public IChangeToken ChangeToken { get; private set; }

        /// <inheritdoc />
        public int MaxLogCount
        {
            get
            {
                int maxLogCount;
                var value = _configuration["MaxLogCount"];
                if (string.IsNullOrEmpty(value))
                {
                    return DefaultMaxLogCount;
                }
                else if (int.TryParse(value, out maxLogCount))
                {
                    return maxLogCount;
                }
                else
                {
                    throw new InvalidOperationException($"Configuration value '{value}' for setting '{nameof(MaxLogCount)}' is not supported.");
                }
            }
        }

        /// <inheritdoc />
        public IMemoryLoggerSettings Reload()
        {
            ChangeToken = null;
            return new MemoryConfigurationLoggerSettings(_configuration);
        }

        /// <inheritdoc />
        public bool TryGetSwitch(string name, out LogLevel level)
        {
            var switches = _configuration.GetSection("LogLevel");
            if (switches == null)
            {
                level = LogLevel.None;
                return false;
            }

            var value = switches[name];
            if (string.IsNullOrEmpty(value))
            {
                level = LogLevel.None;
                return false;
            }
            else if (Enum.TryParse(value, out level))
            {
                return true;
            }
            else
            {
                var message = $"Configuration value '{value}' for category '{name}' is not supported.";
                throw new InvalidOperationException(message);
            }
        }
        #endregion
    }
}
