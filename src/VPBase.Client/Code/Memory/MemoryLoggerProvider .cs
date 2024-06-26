﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using VPBase.Client.Code.Memory.Settings;

namespace VPBase.Client.Code.Memory
{
    public class MemoryLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, MemoryLogger> _loggers = new ConcurrentDictionary<string, MemoryLogger>();

        private readonly Func<string, LogLevel, bool> _filter;

        private readonly Func<LogLevel, string, string, Exception, string> logLineFormatter = null;

        private IMemoryLoggerSettings _settings;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="maxLogCount"></param>
        /// <param name="logLineFormatter"></param>
        public MemoryLoggerProvider(Func<string, LogLevel, bool> filter,
                                        int maxLogCount = 200,
                                        Func<LogLevel, string, string, Exception, string> logLineFormatter = null)
        {
            this.logLineFormatter = logLineFormatter;
            _filter = filter;
            _settings = new MemoryLoggerSettings()
            {
                MaxLogCount = maxLogCount
            };
        }

        /// <summary>
        /// Coustructor with settings object
        /// </summary>
        /// <param name="settings">settings object</param>
        public MemoryLoggerProvider(IMemoryLoggerSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            _settings = settings;

            if (_settings.ChangeToken != null)
            {
                _settings.ChangeToken.RegisterChangeCallback(OnConfigurationReload, null);
            }
        }

        private void OnConfigurationReload(object state)
        {
            // The settings object needs to change here, because the old one is probably holding on 
            // to an old change token. 
            _settings = _settings.Reload();

            foreach (var logger in _loggers.Values)
            {
                logger.Filter = GetFilter(logger.Name, _settings);
            }

            // The token will change each time it reloads, so we need to register again. 
            if (_settings?.ChangeToken != null)
            {
                _settings.ChangeToken.RegisterChangeCallback(OnConfigurationReload, null);
            }
        }

        /// <inheritdoc />
        public ILogger CreateLogger(string name)
        {
            return _loggers.GetOrAdd(name, CreateLoggerImplementation);
        }

        private MemoryLogger CreateLoggerImplementation(string name)
        {
            return new MemoryLogger(name, GetFilter(name, _settings), _settings.MaxLogCount, logLineFormatter);
        }

        private Func<string, LogLevel, bool> GetFilter(string name, IMemoryLoggerSettings settings)
        {
            if (_filter != null)
            {
                return _filter;
            }

            if (settings != null)
            {
                foreach (var prefix in GetKeyPrefixes(name))
                {
                    LogLevel level;
                    if (settings.TryGetSwitch(prefix, out level))
                    {
                        return (n, l) => l >= level;
                    }
                }
            }

            return (n, l) => false;
        }

        private IEnumerable<string> GetKeyPrefixes(string name)
        {
            while (!string.IsNullOrEmpty(name))
            {
                yield return name;
                var lastIndexOfDot = name.LastIndexOf('.');
                if (lastIndexOfDot == -1)
                {
                    yield return "Default";
                    break;
                }
                name = name.Substring(0, lastIndexOfDot);
            }
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
