using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;

using Vardirsoft.Shared.Helpers;

using Vardirsoft.XApp.IoC;
using Vardirsoft.XApp.API;
using Vardirsoft.XApp.Logging;

namespace Vardirsoft.XApp.Application.Configuration
{
    public class ConfigurationManager
    {
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
            if (configs.ContainsKey(config.Key))
            {
                configs[config.Key] = config;
            }
            else
            {
                configs.Add(config.Key, config);
            }
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
                if (configs.ContainsKey(config.Key))
                {
                    configs[config.Key] = config;
                }
                else
                {
                    configs.Add(config.Key, config);
                }
            }
        }
        public void LoadFrom(string path)
        {
            var fileManager = IoCContainer.Resolve<IFileManager>();

            if (fileManager.FileExists(path))
            {
                var fileData = fileManager.TryRead(path);
                if (fileData.HasValue())
                {
                    Dictionary<string, string> configItems = null;
                    try
                    {
                        configItems = JsonConvert.DeserializeObject<Dictionary<string, string>>(fileData);
                    }
                    catch (Exception ex)
                    {
                        IoCContainer.Resolve<ILogger>().PushError(ex, this, "Couldn't load configurations: invalid file syntax");

                        return;
                    }

                    AddOrUpdate(configItems.Select(tuple => new Config(tuple.Key, tuple.Value)));
                }
            }
        }
        public void SaveTo(string path)
        {
            var selected = configs.Select(kvp => kvp.Value)
                                  .Where(config => !config.Temporary)
                                  .ToDictionary(config => config.Key, config => config.Value);
            var data = JsonConvert.SerializeObject(selected, Formatting.Indented);

            IoCContainer.Resolve<IFileManager>().Write(data, path);
        }

        public bool Contains(string key) => configs.ContainsKey(key);
    }
}