using System;
using System.Linq;

namespace MIDE.Helpers
{
    public static class StringHelpers
    {
        public static string Clamp(this string value, int length)
        {
            if (value == string.Empty)
                return value;
            if (length <= value.Length)
                return value;
            return value.Substring(0, length);
        }
        public static (string head, string tail) ExtractUntil(this string value, int start, params char[] endChars)
        {
            if (value == string.Empty)
                return (string.Empty, string.Empty);
            if (start < 0 || start >= value.Length)
                throw new IndexOutOfRangeException();
            for (int i = start; i < value.Length; i++)
            {
                if (endChars.Contains(value[i]))
                {
                    if (i == value.Length - 1)
                        return (value.Substring(start, i - start), string.Empty);
                    return (value.Substring(start, i - start), value.Substring(i + 1, value.Length - i - 1));
                }
            }
            return (value.Substring(start, value.Length - start), string.Empty);
        }
    }
}