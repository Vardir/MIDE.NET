using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

using Newtonsoft.Json;

using Vardirsoft.Shared.Helpers;

using Vardirsoft.XApp.IoC;
using Vardirsoft.XApp.API;
using Vardirsoft.XApp.Helpers;
using Vardirsoft.XApp.Logging;

namespace Vardirsoft.XApp.Application.Configuration
{
    public class ConfigurationManager
    {
        private readonly Dictionary<string, Config> _configs;

        public string this[string key] { [DebuggerStepThrough] get => _configs.TryGetValue(key, out Config config) ? config.Value : null; }

        public ConfigurationManager()
        {
            _configs = new Dictionary<string, Config>();
        }

        public void AddOrUpdate(Config config)
        {
            if (_configs.ContainsKey(config.Key))
            {
                _configs[config.Key] = config;
            }
            else
            {
                _configs.Add(config.Key, config);
            }
        }
        
        public void AddRange(IEnumerable<Config> sequence)
        {
            foreach (var config in sequence)
            {
                Guard.EnsureNot(_configs.ContainsKey(config.Key), typeof(InvalidOperationException), $"Duplicate configuration key entry on '{config.Key}'");
                
                _configs.Add(config.Key, config);
            }
        }
        
        public void AddOrUpdate(IEnumerable<Config> sequence)
        {
            foreach (var config in sequence)
            {
                if (_configs.ContainsKey(config.Key))
                {
                    _configs[config.Key] = config;
                }
                else
                {
                    _configs.Add(config.Key, config);
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
                    Dictionary<string, string> configItems;
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
            var selected = _configs.Select(kvp => kvp.Value)
                                  .Where(config => !config.Temporary)
                                  .ToDictionary(config => config.Key, config => config.Value);
            var data = JsonConvert.SerializeObject(selected, Formatting.Indented);

            IoCContainer.Resolve<IFileManager>().Write(data, path);
        }

        [DebuggerStepThrough]
        public bool Contains(string key) => _configs.ContainsKey(key);
    }
}