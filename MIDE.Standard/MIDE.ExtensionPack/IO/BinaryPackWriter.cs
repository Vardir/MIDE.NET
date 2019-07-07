using System.IO;
using MIDE.ExtensionPack.Helpers;

namespace MIDE.ExtensionPack
{
    /// <summary>
    /// Binary extension pack writer
    /// </summary>
    public class BinaryPackWriter : IPackWriter
    {
        /// <summary>
        /// Writes the given extension pack to the given file
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="path"></param>
        public void WritePack(IExtensionPack pack, string path)
        {
            //TODO: replace with loading actual locals
            var locals = new[]
            {
                ("uk-ua", new PackInfo(pack.Id, "Розширення 1", "Опис", "Примітки")),
                ("en-us", new PackInfo(pack.Id, "Extension 1", "Description", "Release notes"))
            };

            BinaryWriter writer = new BinaryWriter(null);
            writer.Write(pack.Id);
            writer.Write(locals.Length);
            foreach (var local in locals)
            {
                WriteLocals(writer, local.Item1, local.Item2);
            }
            writer.Write(pack.Version.ToString(3));
            writer.Write(pack.Tags);
            writer.Write(pack.Copyright);
            writer.Write(pack.Owners);
            writer.Write(pack.Authors);
            writer.Write(pack.IconBytes);
            writer.Write(pack.AppDependencies, d => d.ToString());
            writer.Write(pack.PackDependencies, d => d.ToString());
            writer.Close();
            writer.Dispose();
        }

        private void WriteLocals(BinaryWriter writer, string lang, PackInfo info)
        {
            writer.Write(lang);
            writer.Write(info.title);
            writer.Write(info.description);
            writer.Write(info.releaseNotes);
        }
    }
}