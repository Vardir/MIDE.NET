namespace MIDE.Application.Configuration
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

        public static implicit operator Config((string key, string value) tuple)
        {
            return new Config(tuple.key, tuple.value);
        }
        public static implicit operator Config((string key, string value, bool temporary) tuple)
        {
            return new Config(tuple.key, tuple.value, tuple.temporary);
        }
        public static implicit operator (string key, string value, bool temporary)(Config config)
        {
            return (config.Key, config.Value, config.Temporary);
        }
    }
}