using System;
using System.Collections.Generic;
using System.IO;

namespace MIDE.ExtensionPack.Helpers
{
    public static class BinaryReaderHelpers
    {
        public static byte[] ReadBytes(this BinaryReader reader)
        {
            int count = reader.ReadInt32();
            byte[] bytes = new byte[count];
            for (int i = 0; i < count; i++)
            {
                bytes[i] = reader.ReadByte();
            }
            return bytes;
        }
        public static string[] ReadStrings(this BinaryReader reader)
        {
            int count = reader.ReadInt32();
            string[] strings = new string[count];
            for (int i = 0; i < count; i++)
            {
                strings[i] = reader.ReadString();
            }
            return strings;
        }
        public static ReadOnlyCollection<T> Read<T>(this BinaryReader reader, Func<string, T> transform)
        {
            int count = reader.ReadInt32();
            List<T> list = new List<T>(count);
            for (int i = 0; i < count; i++)
            {
                list.Add(transform(reader.ReadString()));
            }
            return list.AsReadOnly();
        }
    }
}
