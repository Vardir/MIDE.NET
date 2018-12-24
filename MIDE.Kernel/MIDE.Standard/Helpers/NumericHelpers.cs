namespace MIDE.Standard.Helpers
{
    public static class NumericHelpers
    {
        public static byte Clamp(this byte value, byte min, byte max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
        public static sbyte Clamp(this sbyte value, sbyte min, sbyte max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
        public static uint Clamp(this uint value, uint min, uint max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
        public static short Clamp(this short value, short min, short max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
        public static int Clamp(this int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
        public static long Clamp(this long value, long min, long max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
        public static float Clamp(this float value, float min, float max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
        public static double Clamp(this double value, double min, double max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
        public static decimal Clamp(this decimal value, decimal min, decimal max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
    }
}