using System;
using System.Collections.Generic;

namespace MIDE.Standard.Helpers
{
    public static class CollectionHelpers
    {
        public static int IndexOf<T>(this IList<T> list, Func<T, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            for (int i = 0; i < list.Count; i++)
            {
                if (predicate(list[i]))
                    return i;
            }
            return -1;
        }
    }
}