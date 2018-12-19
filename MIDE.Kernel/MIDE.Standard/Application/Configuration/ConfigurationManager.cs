using System;
using System.Collections.Generic;

namespace MIDE.Standard.Application.Configuration
{
    public class ConfigurationManager
    {
        private static ConfigurationManager instance;
        public static ConfigurationManager Instance => instance ?? (instance = new ConfigurationManager());

        private readonly Dictionary<string, object> configs;

        public object this[string key]
        {
            get
            {
                configs.TryGetValue(key, out object value);
                return value;
            }
        }

        private ConfigurationManager ()
        {
            configs = new Dictionary<string, object>();
        }

        public void AddRange(IEnumerable<Config> sequence)
        {
            foreach (var config in sequence)
            {
                if (configs.ContainsKey(config.Key))
                    throw new InvalidOperationException($"Duplicate configuration key entry on '{config.Key}'");
                configs.Add(config.Key, config.Value);
            }
        }
        public void AddOrUpdate(IEnumerable<Config> sequence)
        {
            foreach (var config in sequence)
            {
                if (!configs.ContainsKey(config.Key))
                    configs.Add(config.Key, config.Value);
                else
                    configs[config.Key] = config.Value;
            }
        }

        public bool Contains(string key) => configs.ContainsKey(key);
    }
}