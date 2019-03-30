using System;
using System.IO;
using System.Reflection;
using MIDE.FileSystem;
using System.Collections.Generic;
using MIDE.Application.Configuration;

namespace MIDE.Kernel.ConsoleTest
{
    public class WinFileManager : FileManager
    {
        public override void MakeFolder(string path)
        {
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }

        public override bool Exists(string path)
        {
            throw new NotImplementedException();
        }
        public override string ExtractName(string path)
        {
            throw new NotImplementedException();
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
                File.Create(filePath);
                File.WriteAllText(filePath, defaultContent);
            }
            return File.ReadAllText(filePath);
        }
        public override IEnumerable<string> EnumerateFiles(string directory, string filter = null)
        {
            if (!Directory.Exists(directory))
                throw new ArgumentException($"Directory not found [{directory}]");
            return Directory.EnumerateFiles(directory, filter);
        }
        public override IEnumerable<(string prop, string val)> ExtractProperties(string path)
        {
            throw new NotImplementedException();
        }
        
        public override void Write(string data, string path)
        {
            throw new NotImplementedException();
        }

        public override void Write(string[] data, string path)
        {
            throw new NotImplementedException();
        }
    }
}