using System;
using System.Collections;
using System.Collections.Generic;

namespace MIDE.WPFApp.Helpers
{
    public static class CollectionHelpers
    {
        public static IEnumerable<T> Transform<T>(this IEnumerable collection, Func<object, T> transform)
        {
            foreach (var obj in collection)
                yield return transform(obj);
        }
    }
}