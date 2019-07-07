using System;
using System.Collections.Generic;
using System.IO;

namespace MIDE.ExtensionPack.Helpers
{
    public static class BinaryWriterHelpers
    {
        public static BinaryWriter Write(this BinaryWriter writer, byte[] array)
        {
            writer.Write(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                writer.Write(array[i]);
            }
            return writer;
        }
        public static BinaryWriter Write(this BinaryWriter writer, string[] array)
        {
            writer.Write(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                writer.Write(array[i]);
            }
            return writer;
        }
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
