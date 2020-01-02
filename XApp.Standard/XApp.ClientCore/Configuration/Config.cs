using System.Diagnostics;

namespace Vardirsoft.XApp.Application.Configuration
{
    public struct Config
    {
        public readonly bool Temporary;
        public readonly string Key;
        public readonly string Value;

        public Config(string key, string value)
        {
            Key = key;
            Value = value;
            Temporary = false;
        }
        
        public Config(string key, string value, bool temporary)
        {
            Key = key;
            Value = value;
            Temporary = temporary;
        }

        [DebuggerStepThrough]
        public static implicit operator Config((string key, string value) tuple) => new Config(tuple.key, tuple.value);

        [DebuggerStepThrough]
        public static implicit operator Config((string key, string value, bool temporary) tuple) => new Config(tuple.key, tuple.value, tuple.temporary);

        [DebuggerStepThrough]
        public static implicit operator (string key, string value, bool temporary)(Config config) => (config.Key, config.Value, config.Temporary);
    }
}