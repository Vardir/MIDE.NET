using System;

namespace XApp.Helpers
{
    public static class EnumHelpers
    {
        public static bool HasKey<T>(string value, out T key)
            where T: struct
        {
            var type = typeof(T);
            key = default;

            if (!type.IsEnum)
                throw new ArgumentException("The specified argument is not an Enum subtype", nameof(type));

            if (string.IsNullOrWhiteSpace(value))
                return false;

            var names = Enum.GetNames(type);
            var compareTo = value.ToLower();

            for (int i = 0; i < names.Length; i++)
            {
                var name = names[i].ToLower();
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