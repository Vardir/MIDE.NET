using System;

namespace MIDE.Helpers
{
    public static class EnumHelpers
    {
        public static bool HasKey<T>(string value, out T key)
            where T: struct
        {
            Type type = typeof(T);
            key = default;
            if (!type.IsEnum)
                throw new ArgumentException("The specified argument is not an Enum subtype", nameof(type));
            if (string.IsNullOrWhiteSpace(value))
                return false;
            string[] names = Enum.GetNames(type);
            string compareTo = value.ToLower();
            for (int i = 0; i < names.Length; i++)
            {
                string name = names[i].ToLower();
                if (name == compareTo)
                {
                    Enum.TryParse(names[i], out key);
                    return true;
                }
            }
            return false;
        }
    }
}