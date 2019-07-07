using System.IO;
using MIDE.ExtensionPack.Helpers;
using System.Collections.Generic;

namespace MIDE.ExtensionPack
{
    public class BinaryPackReader : IPackReader
    {
        public XPack Read(string path)
        {
            BinaryReader reader = new BinaryReader(null);
            string id = reader.ReadString();
            var locals = ReadLocals(reader);
            string ver = reader.ReadString();
            var tags = reader.ReadStrings();
            string copy = reader.ReadString();
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
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string local = reader.ReadString();
                string title = reader.ReadString();
                string descr = reader.ReadString();
                string notes = reader.ReadString();
                yield return (local, new PackInfo(null, title, descr, notes));
            }
        }
    }
}