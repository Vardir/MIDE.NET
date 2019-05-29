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
        /// <summary>
        /// Extracts a substring starting from the given index until it reaches one of the given separators.
        /// <para></para>
        /// Returns a tuple (head, tail), where head - substring before the separator, 
        /// tail - the rest of the string starting from the next character after separator
        /// </summary>
        /// <param name="value"></param>
        /// <param name="start"></param>
        /// <param name="endChars"></param>
        /// <returns></returns>
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