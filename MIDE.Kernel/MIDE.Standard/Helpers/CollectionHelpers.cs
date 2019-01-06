using System;
using System.Collections.Generic;

namespace MIDE.Helpers
{
    public static class CollectionHelpers
    {
        /// <summary>
        /// Checks whether the given collection contains an element that matches the predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool Contains<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            foreach (var item in collection)
            {
                if (predicate(item))
                    return true;
            }
            return false;
        }
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

        /// <summary>
        /// Checks if another collection contains any of the items from the current one
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="another"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool AnyIn<T>(this IEnumerable<T> collection, IEnumerable<T> another)
            where T: IComparable<T>
        {
            if (another == null)
                throw new ArgumentNullException(nameof(another));

            foreach (var item in collection)
            {
                foreach (var item2 in another)
                {
                    if (item.CompareTo(item2) == 0)
                        return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Checks if another collection contains any of the items from the current one transformed with the provided function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Y"></typeparam>
        /// <param name="collection"></param>
        /// <param name="extractor"></param>
        /// <param name="another"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool AnyIn<T, Y>(this IEnumerable<T> collection, Func<T, Y> extractor, IEnumerable<Y> another)
            where Y : IComparable<Y>
        {
            if (another == null)
                throw new ArgumentNullException(nameof(another));
            if (extractor == null)
                throw new ArgumentNullException(nameof(extractor));

            foreach (var item in collection)
            {
                foreach (var item2 in another)
                {
                    var extracted = extractor(item);
                    if (extracted.CompareTo(item2) == 0)
                        return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Checks if another collection, transformed with the provided function, contains any of the items from the current one
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Y"></typeparam>
        /// <param name="collection"></param>
        /// <param name="extractor"></param>
        /// <param name="another"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool AnyIn<T, Y>(this IEnumerable<T> collection, IEnumerable<Y> another, Func<Y, T> extractor)
            where T : IComparable<T>
        {
            if (another == null)
                throw new ArgumentNullException(nameof(another));
            if (extractor == null)
                throw new ArgumentNullException(nameof(extractor));

            foreach (var item in collection)
            {
                foreach (var item2 in another)
                {
                    var extracted = extractor(item2);
                    if (extracted.CompareTo(item) == 0)
                        return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Checks if another collection does not contain any of the items from the current one
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="another"></param>
        /// <returns></returns>
        public static bool AnyNotIn<T>(this IEnumerable<T> collection, IEnumerable<T> another)
            where T: IComparable<T>
        {
            if (another == null)
                throw new ArgumentNullException(nameof(another));

            bool found = false;
            foreach (var item in collection)
            {
                foreach (var item2 in another)
                {
                    if (item.CompareTo(item2) == 0)
                        found = true;
                }
            }
            return !found;
        }
        /// <summary>
        /// Checks if another collection does not contain any of the items from the current one transformed with the provided function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Y"></typeparam>
        /// <param name="collection"></param>
        /// <param name="extractor"></param>
        /// <param name="another"></param>
        /// <returns></returns>
        public static bool AnyNotIn<T, Y>(this IEnumerable<T> collection, Func<T, Y> extractor, IEnumerable<Y> another)
            where Y : IComparable<Y>
        {
            if (another == null)
                throw new ArgumentNullException(nameof(another));
            if (extractor == null)
                throw new ArgumentNullException(nameof(extractor));

            bool found = false;
            foreach (var item in collection)
            {
                foreach (var item2 in another)
                {
                    var extracted = extractor(item);
                    if (extracted.CompareTo(item2) == 0)
                        found = true;
                }
            }
            return !found;
        }
        /// <summary>
        /// Checks if another collection, transformed with the provided function, does not contain any of the items from the current one
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Y"></typeparam>
        /// <param name="collection"></param>
        /// <param name="extractor"></param>
        /// <param name="another"></param>
        /// <returns></returns>
        public static bool AnyNotIn<T, Y>(this IEnumerable<T> collection, IEnumerable<Y> another, Func<Y, T> extractor)
            where T : IComparable<T>
        {
            if (another == null)
                throw new ArgumentNullException(nameof(another));
            if (extractor == null)
                throw new ArgumentNullException(nameof(extractor));

            bool found = false;
            foreach (var item in collection)
            {
                foreach (var item2 in another)
                {
                    var extracted = extractor(item2);
                    if (extracted.CompareTo(item) == 0)
                        found = true;
                }
            }
            return !found;
        }
        /// <summary>
        /// Verifies if the given index is out of range for the current collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool OutOfRange<T>(this IList<T> list, int index) => index < 0 && index >= list.Count;
        /// <summary>
        /// Verifies if the given index is out of range for the current array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool OutOfRange<T>(this T[] array, int index) => index < 0 || index >= array.Length;
    }
}