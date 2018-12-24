using System;
using System.Collections.Generic;

namespace MIDE.Standard.Helpers
{
    public static class CollectionHelpers
    {
        /// <summary>
        /// Finds the first occurrence of the item by the given predicate and gives out it's index. Returns -1 if none found
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
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
        /// <summary>
        /// LINQ Select implementation for collections to produce array with transformed items
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="collection"></param>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static TResult[] Select<T, TResult>(this ICollection<T> collection, Func<T, TResult> transform)
        {
            if (transform == null)
                throw new ArgumentNullException(nameof(transform));
            TResult[] result = new TResult[collection.Count];
            int i = 0;
            foreach (var item in collection)
                result[i++] = transform(item);
            return result;
        }
        /// <summary>
        /// Searches for the first occurrence of the element by the given predicate and returns the value extracted from it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <param name="extractor"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static TResult FirstWith<T, TResult>(this IEnumerable<T> collection, Func<T, bool> predicate, Func<T, TResult> extractor)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            if (extractor == null)
                throw new ArgumentNullException(nameof(extractor));
            foreach (var item in collection)
            {
                if (predicate(item))
                    return extractor(item);
            }
            return default;
        }
        /// <summary>
        /// Searches for the first occurrence of the element by the given predicate and returns the index of the item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <param name="extractor"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static int FirstIndexWith<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            int i = 0;
            foreach (var item in collection)
            {
                if (predicate(item))
                    return i;
                i++;
            }
            return -1;
        }
        /// <summary>
        /// Searches for the last occurrence of the element by the given predicate and returns the value extracted from it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <param name="extractor"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static TResult LastWith<T, TResult>(this IEnumerable<T> collection, Func<T, bool> predicate, Func<T, TResult> extractor)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            if (extractor == null)
                throw new ArgumentNullException(nameof(extractor));
            T last = default;
            bool any = false;
            foreach (var item in collection)
            {
                if (predicate(item))
                {
                    last = item;
                    any = true;
                }
            }
            if (!any)
                return default;
            return extractor(last);
        }
        /// <summary>
        /// Searches for the last occurrence of the element by the given predicate and returns the index of the item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <param name="extractor"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static int LastIndexWith<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            int i = 0;
            int last = -1;
            foreach (var item in collection)
            {
                if (predicate(item))
                    last = i;
                i++;
            }
            return last;
        }
    }
}