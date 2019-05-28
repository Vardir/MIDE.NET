using System;
using System.IO;
using MIDE.FileSystem;
using System.Collections.Generic;

namespace MIDE.WPF.FileSystem
{
    public class WinFileResolver : DefaultPathResolver
    {
        public override IEnumerable<(string prop, string val)> ExtractProperties(string path)
        {
            var file = new FileInfo(path);
            if (file.Exists)
            {
                yield return ("Attributes", file.Attributes.ToString());
                yield return ("Creation time UTC", file.CreationTimeUtc.ToString());
                yield return ("Parent directory", file.DirectoryName);
                yield return ("Extension", file.Extension);
                yield return ("Is readonly", file.IsReadOnly.ToString());
                yield return ("Last access time UTC", file.LastAccessTimeUtc.ToString());
                yield return ("Last write time UTC", file.LastWriteTimeUtc.ToString());
                yield return ("Length", file.Length.ToString());
                yield break;
            }

            var directory = new DirectoryInfo(path);
            if (directory.Exists)
            {
                yield return ("Attributes", directory.Attributes.ToString());
                yield return ("Creation time UTC", directory.CreationTimeUtc.ToString());
                yield return ("Parent directory", directory.Parent.FullName);
                yield return ("Last access time UTC", directory.LastAccessTimeUtc.ToString());
                yield return ("Last write time UTC", directory.LastWriteTimeUtc.ToString());
                yield break;
            }

            throw new ArgumentException("The given path is invalid", nameof(path));
        }
    }
}