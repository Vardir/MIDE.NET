using System;
using System.Collections;
using MIDE.API.Components;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MIDE.Helpers
{
    public static class CollectionHelpers
    {
        /// <summary>
        /// Adds a range of items to an existing collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="items"></param>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            foreach (var item in items)
                collection.Add(item);
        }
        /// <summary>
        /// Ensures the count of elements in collection is equals to the given count. 
        /// Uses the generator to create new elements if collection needs to be enlarged
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="generator"></param>
        /// <param name="count"></param>
        public static void EnsureCount<T>(this IList<T> list, Func<T> generator, int count)
        {
            if (count < 0)
                throw new ArgumentException("Expected count greater or equals to 0");
            if (count < list.Count)
            {
                int diff = list.Count - count;
                for (int i = 0; i < diff; i++)
                    list.RemoveAt(list.Count - 1);
            }
            else if (count > list.Count)
            {
                int diff = count - list.Count;
                for (int i = 0; i < diff; i++)
                    list.Add(generator());
            }
        }
        /// <summary>
        /// Inserts an item into collection based on it's ordinal index
        /// </summary>
        /// <param name="items"></param>
        /// <param name="item"></param>
        /// <param name="minOrdinal"></param>
        /// <param name="maxOrdinal"></param>
        public static void Insert<T>(this IList<T> items, T item)
            where T: IOrderable
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (item.OrdinalIndex <= 0)
            {
                int firstIndex = items.LastIndexWith(i => i.OrdinalIndex <= item.OrdinalIndex);
                if (firstIndex == -1)
                    items.Insert(0, item);
                else
                    items.Insert(firstIndex + 1, item);
            }
            else
            {
                int firstIndex = items.FirstIndexWith(i => i.OrdinalIndex >= item.OrdinalIndex);
                if (firstIndex == -1)
                    items.Add(item);
                else
                    items.Insert(firstIndex, item);
            }
        }
        /// <summary>
        /// Moves an item from the specified index to destination
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="origin"></param>
        /// <param name="destination"></param>
        public static void MoveTo<T>(this IList<T> list, int origin, int destination)
        {
            if (list.OutOfRange(origin) || list.OutOfRange(destination))
                throw new IndexOutOfRangeException();
            T item = list[origin];
            for (int i = origin; i <= destination; i++)
            {
                list[i] = list[i + 1];
            }
            list[destination] = item;
        }
        /// <summary>
        /// Iterates through collection and applies action to each item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            foreach (var item in collection)
            {
                action(item);
            }
        }
        /// <summary>
        /// Iterates through collection and applies action to each item using it's index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="action"></param>
        public static void ForEachI<T>(this IEnumerable<T> collection, Action<int, T> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            int i = 0;
            foreach (var item in collection)
            {
                action(i, item);
                i++;
            }
        }
        /// <summary>
        /// Removes all the items that match predicate from the given collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="predicate"></param>
        public static void Remove<T>(this LinkedList<T> list, Func<T, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            var node = list.First;
            while (node != null)
            {
                var next = node.Next;
                if (predicate(node.Value))
                    list.Remove(node);
                node = next;
            }
        }

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
        /// Finds the first occurrence of the item starting from the given index by the given predicate and gives out it's index. Returns -1 if none found
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static int IndexOf<T>(this IList<T> list, Func<T, bool> predicate, int startIndex)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            if (list.OutOfRange(startIndex))
                throw new IndexOutOfRangeException();
            for (int i = startIndex; i < list.Count; i++)
            {
                if (predicate(list[i]))
                    return i;
            }
            return -1;
        }
        /// <summary>
        /// Searches for item in collection that matches the given predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T Find<T>(this Collection<T> collection, Func<T, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            foreach (var item in collection)
            {
                if (predicate(item))
                    return item;
            }
            return default;
        }
        /// <summary>
        /// Searches for node in linked list which value matches the given predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static LinkedListNode<T> Find<T>(this LinkedList<T> list, Func<T, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            var node = list.First;
            while (node != null)
            {
                if (predicate(node.Value))
                    return node;
                node = node.Next;
            }
            return null;
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
        /// LINQ Select implementation for collections to produce array with transformed items
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="collection"></param>
        /// <param name="transform">A transformation function that provides item itself and it's index</param>
        /// <returns></returns>
        public static TResult[] Select<T, TResult>(this ICollection<T> collection, Func<int, T, TResult> transform)
        {
            if (transform == null)
                throw new ArgumentNullException(nameof(transform));
            TResult[] result = new TResult[collection.Count];
            int i = 0;
            foreach (var item in collection)
                result[i++] = transform(i, item);
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
        /// Searches for the first occurrence of the element by the given predicate and returns the value extracted from it
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <param name="extractor"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static TResult FirstWith<TResult>(this IList collection, Func<object, bool> predicate, Func<object, TResult> extractor)
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
        /// Finds indexes of all the elements in collection that are match the predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static int[] IndexesOf<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            int count = 0;
            var selected = select();
            return selected.ToArray(count);

            IEnumerable<int> select()
            {
                int index = 0;
                foreach (var element in collection)
                {
                    if (predicate(element))
                    {
                        count++;
                        yield return index;
                    }
                    index++;
                }
            }
        }

        /// <summary>
        /// Generates an array from IEnumerable of the known size. 
        /// <para>
        /// If count of elements in collection is bigger than given <paramref name="count"/> parameter, throws an exception.
        /// </para>
        /// <para>
        /// If count of elements in collection is lesser than given <paramref name="count"/> parameter, not used space in array will hold defaults of <seealso cref="T"/>
        /// </para>
        /// </summary>
        /// <typeparam name="T">Type of elements in collection</typeparam>
        /// <param name="collection">The collection to transform into array</param>
        /// <param name="count">Count of elements to be stored in the array</param>
        /// <returns>Array of elements</returns>
        /// <exception cref="IndexOutOfRangeException">Throws if count of elements in collection was bigger than array capacity</exception>
        public static T[] ToArray<T>(this IEnumerable<T> collection, int count)
        {
            T[] array = new T[count];
            int index = 0;
            foreach (var element in collection)
                array[index++] = element;
            return array;
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
        public static bool OutOfRange<T>(this IList<T> list, int index) => index < 0 || index >= list.Count;
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