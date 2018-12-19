namespace MIDE.Standard.Application.Configuration
{
    public struct Config
    {
        public readonly string Key;
        public readonly object Value;

        public Config(string key, object value)
        {
            Key = key;
            Value = value;
        }
    }
}