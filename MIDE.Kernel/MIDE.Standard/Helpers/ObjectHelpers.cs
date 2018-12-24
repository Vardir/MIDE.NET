using System;

namespace MIDE.Standard.Helpers
{
    public static class ObjectHelpers
    {
        public static bool BothEquals(this object value, object first, object second)
        {
            return value.Equals(first) && value.Equals(second);
        }
        public static bool BothEquals<T>(this T value, T first, T second)
            where T: IEquatable<T>
        {
            return value.Equals(first) && value.Equals(second);
        }
        public static bool BothEquals2<Y>(this Y value, Y first, Y second)
            where Y : IComparable<Y>
        {
            return value.Equals(first) && value.Equals(second);
        }
    }
}