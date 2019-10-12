using System;
using System.IO;
using System.Collections.Generic;

namespace Vardirsoft.XApp.ExtensionPack.Helpers
{
    /// <summary>
    /// A set of helper methods for <see cref="BinaryWriter"/>
    /// </summary>
    public static class BinaryWriterHelpers
    {
        /// <summary>
        /// Writes an array of bytes to binary stream starting with an integer value to indicate array's length
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static BinaryWriter Write(this BinaryWriter writer, byte[] array)
        {
            writer.Write(array.Length);

            for (int i = 0; i < array.Length; i++)
            {
                writer.Write(array[i]);
            }

            return writer;
        }
        /// <summary>
        /// Writes an array of strings to binary stream starting with an integer value to indicate array's length
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static BinaryWriter Write(this BinaryWriter writer, string[] array)
        {
            writer.Write(array.Length);

            for (int i = 0; i < array.Length; i++)
            {
                writer.Write(array[i]);
            }

            return writer;
        }
        /// <summary>
        /// Writes the given collection to binary stream transforming each item to string using the given transform function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="writer"></param>
        /// <param name="collection"></param>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static BinaryWriter Write<T>(this BinaryWriter writer, ICollection<T> collection, Func<T, string> transform)
        {
            writer.Write(collection.Count);
            
            foreach (var item in collection)
            {
                writer.Write(transform(item));
            }
            
            return writer;
        }
    }
}