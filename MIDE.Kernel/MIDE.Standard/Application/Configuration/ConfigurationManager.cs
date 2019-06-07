using System;
using System.Linq;
using MIDE.FileSystem;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MIDE.Application.Configuration
{
    public class ConfigurationManager
    {
        private static ConfigurationManager instance;
        public static ConfigurationManager Instance => instance ?? (instance = new ConfigurationManager());

        private readonly Dictionary<string, Config> configs;

        public string this[string key]
        {
            get
            {
                if (configs.TryGetValue(key, out Config config))
                    return config.Value;
                return null;
            }
        }

        private ConfigurationManager()
        {
            configs = new Dictionary<string, Config>();
        }

        public void AddOrUpdate(Config config)
        {
            if (!configs.ContainsKey(config.Key))
                configs.Add(config.Key, config);
            else
                configs[config.Key] = config;
        }
        public void AddRange(IEnumerable<Config> sequence)
        {
            foreach (var config in sequence)
            {
                if (configs.ContainsKey(config.Key))
                    throw new InvalidOperationException($"Duplicate configuration key entry on '{config.Key}'");
                configs.Add(config.Key, config);
            }
        }
        public void AddOrUpdate(IEnumerable<Config> sequence)
        {
            foreach (var config in sequence)
            {
                if (!configs.ContainsKey(config.Key))
                    configs.Add(config.Key, config);
                else
                    configs[config.Key] = config;
            }
        }
        public void LoadFrom(string path)
        {
            if (!FileManager.FileExists(path))
                return;
            string fileData = FileManager.TryRead(path);
            if (fileData == null)
                return;
            Dictionary<string, string> configItems = null;
            try
            {
                configItems = JsonConvert.DeserializeObject<Dictionary<string, string>>(fileData);
            }
            catch (Exception ex)
            {
                return;
            }
            AddOrUpdate(configItems.Select(tuple => new Config(tuple.Key, tuple.Value)));
        }
        public void SaveTo(string path)
        {
            var selected = configs.Select(kvp => kvp.Value)
                                  .Where(config => !config.Temporary)
                                  .ToDictionary(config => config.Key, config => config.Value);
            string data = JsonConvert.SerializeObject(selected, Formatting.Indented);
            FileManager.Write(data, path);
        }

        public bool Contains(string key) => configs.ContainsKey(key);
    }
}