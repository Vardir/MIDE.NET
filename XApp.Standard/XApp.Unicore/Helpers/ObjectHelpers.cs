using System;

namespace XApp.Helpers
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
        public static bool IsNot<T>(this object obj) => !(obj is T);

        /// <summary>
        /// Verifies if the reference to an object is not null
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool HasValue(this object obj) => obj != null;
    }
}