using System;
using System.IO;
using MIDE.FileSystem;
using System.Collections.Generic;
using System.Text;

namespace MIDE.WPFApp.FileSystem
{
    public class WinFileManager : FileManager
    {
        public void MakeFolder(string path)
        {
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }

        public override string MapPath(string path)
        {
            throw new NotImplementedException();
        }
        public override string TryRead(string filePath)
        {
            if (!File.Exists(filePath))
                return null;
            return File.ReadAllText(filePath);
        }
        public override string ReadOrCreate(string filePath, string defaultContent = "")
        {
            if (!File.Exists(filePath))
            {
                using (FileStream fs = File.Create(filePath))
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(defaultContent);
                    fs.Write(bytes, 0, bytes.Length);
                    return defaultContent;
                }
            }
            return File.ReadAllText(filePath);
        }
        public override IEnumerable<string> EnumerateFiles(string directory, string filter = null)
        {
            if (!Directory.Exists(directory))
                throw new ArgumentException($"Directory not found [{directory}]");
            return Directory.EnumerateFiles(directory, filter);
        }
    }
}