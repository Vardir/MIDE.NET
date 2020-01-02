using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Vardirsoft.XApp.ExtensionPack.Helpers
{
    /// <summary>
    /// A set of helper methods for <see cref="BinaryReader"/>
    /// </summary>
    public static class BinaryReaderHelpers
    {
        /// <summary>
        /// Reads an array of bytes from binary stream, expects an integer value given before bytes to define length of array
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static byte[] ReadBytes(this BinaryReader reader)
        {
            var count = reader.ReadInt32();
            var bytes = new byte[count];

            for (var i = 0; i < count; i++)
            {
                bytes[i] = reader.ReadByte();
            }

            return bytes;
        }
        /// <summary>
        /// Reads an array of strings from binary stream, expects an integer value given before the strings to define length of array
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static string[] ReadStrings(this BinaryReader reader)
        {
            var count = reader.ReadInt32();
            var strings = new string[count];

            for (var i = 0; i < count; i++)
            {
                strings[i] = reader.ReadString();
            }

            return strings;
        }
        /// <summary>
        /// Reads a collection of items written in form of strings using the given transforms function,
        /// expects an integer value given before the strings to define length of collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static ReadOnlyCollection<T> Read<T>(this BinaryReader reader, Func<string, T> transform)
        {
            var count = reader.ReadInt32();
            var list = new List<T>(count);

            for (var i = 0; i < count; i++)
            {
                list.Add(transform(reader.ReadString()));
            }
            
            return list.AsReadOnly();
        }
    }
}