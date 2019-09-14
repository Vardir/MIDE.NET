using System;

namespace XApp.Helpers
{
    public static class NumericHelpers
    {
        public static bool IsInRange(this byte value, byte min, byte max, RangeFlags range = RangeFlags.None)
        {
            var left = value > min || (range.HasFlag(RangeFlags.LeftInclusive) && value == min);
            var right = value < max || (range.HasFlag(RangeFlags.RightInclusive) && value == max);
            return left && right;
        }
        public static bool IsInRange(this sbyte value, sbyte min, sbyte max, RangeFlags range = RangeFlags.None)
        {
            var left = value > min || (range.HasFlag(RangeFlags.LeftInclusive) && value == min);
            var right = value < max || (range.HasFlag(RangeFlags.RightInclusive) && value == max);
            return left && right;
        }
        public static bool IsInRange(this uint value, uint min, uint max, RangeFlags range = RangeFlags.None)
        {
            var left = value > min || (range.HasFlag(RangeFlags.LeftInclusive) && value == min);
            var right = value < max || (range.HasFlag(RangeFlags.RightInclusive) && value == max);
            return left && right;
        }
        public static bool IsInRange(this short value, short min, short max, RangeFlags range = RangeFlags.None)
        {
            var left = value > min || (range.HasFlag(RangeFlags.LeftInclusive) && value == min);
            var right = value < max || (range.HasFlag(RangeFlags.RightInclusive) && value == max);
            return left && right;
        }
        public static bool IsInRange(this int value, int min, int max, RangeFlags range = RangeFlags.None)
        {
            var left = value > min || (range.HasFlag(RangeFlags.LeftInclusive) && value == min);
            var right = value < max || (range.HasFlag(RangeFlags.RightInclusive) && value == max);
            return left && right;
        }
        public static bool IsInRange(this long value, long min, long max, RangeFlags range = RangeFlags.None)
        {
            var left = value > min || (range.HasFlag(RangeFlags.LeftInclusive) && value == min);
            var right = value < max || (range.HasFlag(RangeFlags.RightInclusive) && value == max);
            return left && right;
        }
        public static bool IsInRange(this float value, float min, float max, RangeFlags range = RangeFlags.None)
        {
            var left = value > min || (range.HasFlag(RangeFlags.LeftInclusive) && value == min);
            var right = value < max || (range.HasFlag(RangeFlags.RightInclusive) && value == max);
            return left && right;
        }
        public static bool IsInRange(this double value, double min, double max, RangeFlags range = RangeFlags.None)
        {
            var left = value > min || (range.HasFlag(RangeFlags.LeftInclusive) && value == min);
            var right = value < max || (range.HasFlag(RangeFlags.RightInclusive) && value == max);
            return left && right;
        }
        public static bool IsInRange(this decimal value, decimal min, decimal max, RangeFlags range = RangeFlags.None)
        {
            var left = value > min || (range.HasFlag(RangeFlags.LeftInclusive) && value == min);
            var right = value < max || (range.HasFlag(RangeFlags.RightInclusive) && value == max);
            return left && right;
        }

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

    [Flags]
    public enum RangeFlags
    {
        None,
        LeftInclusive = 1,
        RightInclusive = 2,
        BothInclusive = LeftInclusive | RightInclusive
    }
}