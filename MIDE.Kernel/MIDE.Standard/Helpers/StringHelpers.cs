namespace MIDE.Standard.Helpers
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
    }
}