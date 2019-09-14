using System.IO;
using System.Collections.Generic;

using XApp.ExtensionPack.Helpers;

namespace XApp.ExtensionPack
{
    /// <summary>
    /// Binary extension pack reader
    /// </summary>
    public class BinaryPackReader : IPackReader
    {
        /// <summary>
        /// Reads an extension pack from the given binary file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IExtensionPack Read(string path)
        {
            var reader = new BinaryReader(null);
            var id = reader.ReadString();
            var locals = ReadLocals(reader);
            var ver = reader.ReadString();
            var tags = reader.ReadStrings();
            var copy = reader.ReadString();
            var owners = reader.ReadStrings();
            var authors = reader.ReadStrings();
            var icon = reader.ReadBytes();
            var appdeps = reader.Read(s => Dependency.FromString(s));
            var packdeps = reader.Read(s => Dependency.FromString(s));

            reader.Close();
            reader.Dispose();

            return null;
        }

        private IEnumerable<(string local, PackInfo info)> ReadLocals(BinaryReader reader)
        {
            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var local = reader.ReadString();
                var title = reader.ReadString();
                var descr = reader.ReadString();
                var notes = reader.ReadString();
                
                yield return (local, new PackInfo(null, title, descr, notes));
            }
        }
    }
}