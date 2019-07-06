using System.Collections.Generic;

namespace MIDE.Application.Localization
{
    public sealed class LocalizationNamespace
    {
        private readonly Dictionary<string, string> dictionary;

        public string this[string key]
        {
            get
            {
                if (dictionary.TryGetValue(key, out string value))
                    return value;
                return key;
            }
        }

        public LocalizationNamespace()
        {
            dictionary = new Dictionary<string, string>();
        }

        public void Add(string key, string value)
        {
            if (dictionary.ContainsKey(key))
                return;
            dictionary.Add(key, value);
        }
        public void AddOrUpdate(string key, string value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
                return;
            }
            dictionary.Add(key, value);
        }

        public string GetString(string key) => dictionary[key];
        public string GetString(string key, string defaultValue = "")
        {
            if (dictionary.TryGetValue(key, out string value))
                return value;
            return defaultValue;
        }
    }
}