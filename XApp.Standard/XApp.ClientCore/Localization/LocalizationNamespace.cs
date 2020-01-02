using System.Collections.Generic;

namespace Vardirsoft.XApp.Application.Localization
{
    public sealed class LocalizationNamespace
    {
        private readonly Dictionary<string, string> _dictionary;

        public string this[string key] => _dictionary.TryGetValue(key, out var value) ? value : key;

        public LocalizationNamespace()
        {
            _dictionary = new Dictionary<string, string>();
        }

        public void Add(string key, string value)
        {
            if (_dictionary.ContainsKey(key))
                return;

            _dictionary.Add(key, value);
        }
        public void AddOrUpdate(string key, string value)
        {
            if (_dictionary.ContainsKey(key))
            {
                _dictionary[key] = value;

                return;
            }

            _dictionary.Add(key, value);
        }

        public string GetString(string key) => _dictionary[key];
        public string GetString(string key, string defaultValue) => _dictionary.TryGetValue(key, out var value) ? value : defaultValue;
    }
}