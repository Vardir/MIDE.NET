using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

using XApp.API;
using XApp.IoC;
using XApp.Helpers;
using XApp.Logging;

namespace XApp.FileSystem
{
    public class FileManager : IFileManager
    {
        /// <summary>
        /// Recursively deletes a directory from a specified path
        /// </summary>
        /// <param name="path"></param>
        public void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                var dir = new DirectoryInfo(path);
                foreach (var file in dir.GetFiles())
                {
                    File.Delete(file.FullName);
                }

                foreach (var subdir in dir.GetDirectories())
                {
                    DeleteDirectory(subdir.FullName);
                }

                Directory.Delete(path);
            }
        }

        /// <summary>
        /// Writes the given string to the given file
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        public void Write(string data, string path) => File.WriteAllText(path, data);

        /// <summary>
        /// Writes the given lines of text to the given file
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        public void Write(string[] data, string path) => File.WriteAllLines(path, data);
        public void Write(IEnumerable<string> data, string path)
        {
            using (var writer = new StreamWriter(File.OpenWrite(path)))
            {
                foreach (var line in data)
                {
                    writer.WriteLine(line);
                }
            }
        }

        /// <summary>
        /// Copies all the files and subdirectories from the source directory to the given destination
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public void Copy(string source, string destination)
        {
            if (Directory.Exists(source))
            {                
                if (!Directory.Exists(destination))
                {
                    Directory.CreateDirectory(destination);
                }

                var dir = new DirectoryInfo(source);
                foreach (var file in  dir.GetFiles())
                {
                    string temppath = Path.Combine(destination, file.Name);
                    file.CopyTo(temppath, false);
                }

                foreach (var subdir in dir.GetDirectories())
                {
                    string temppath = Path.Combine(destination, subdir.Name);
                    Copy(subdir.FullName, temppath);
                }
            }
        }

        /// <summary>
        /// Tries to serialize the given object in file, if it is impossible writes log
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        public void Serialize(object data, string path)
        {
            if (data == null)
                File.WriteAllText(path, "<null>");
                
            var type = data.GetType();
            if (type.HasAttribute<SerializableAttribute>())
            {
                var formatter = new BinaryFormatter();
                try
                {
                    using (var fs = new FileStream(path, FileMode.OpenOrCreate))
                    {
                        formatter.Serialize(fs, data);
                    }
                }
                catch (Exception ex)
                {
                    IoCContainer.Resolve<ILogger>().PushError(ex, formatter, "Can not serialize data");
                }
            }
            else             
            {
                File.WriteAllText(path, data.ToString());
            }
        }

        /// <summary>
        /// Cleans all the contents of the given directory
        /// </summary>
        /// <param name="path"></param>
        public void CleanDirectory(string path)
        {
            if (DirectoryExists(path))
            {
                var dir = new DirectoryInfo(path);
                var dirs = dir.GetDirectories();
                var files = dir.GetFiles();

                foreach (var file in files)
                {
                    Delete(file.FullName);
                }
                foreach (var subdir in dirs)
                {
                    DeleteDirectory(subdir.FullName);
                }
            }
        }

        public string GetAbsolutePath(string path) => Path.GetFullPath(path);
        public string GetParentDirectory(string path) => Path.GetDirectoryName(path);

        /// <summary>
        /// Verifies if the given path exists, it can be either file or directory
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool Exists(string path) => File.Exists(path) || Directory.Exists(path);

        /// <summary>
        /// Verifies if the given path is existing file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool FileExists(string path) => File.Exists(path);

        /// <summary>
        /// Verifies if the given path is existing directory
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool DirectoryExists(string path) => Directory.Exists(path);

        /// <summary>
        /// Verifies if the given path is file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool IsFile(string path) => Path.HasExtension(path);

        /// <summary>
        /// Tries to load bytes from the given file and returns null if file does not exist
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public byte[] TryReadBytes(string path)
        {
            if (File.Exists(path))
                return File.ReadAllBytes(path);

            return null;
        }
        public string GetFileParent(string file)
        {
            if (File.Exists(file))
                return Path.GetDirectoryName(file);

            return null;
        }

        /// <summary>
        /// Tries to load data from the given file and returns null if file does not exist
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public string TryRead(string filePath)
        {
            if (File.Exists(filePath))
                return File.ReadAllText(filePath);

            return null;
        }

        /// <summary>
        /// Tries to load data from the given file and returns null if file does not exist
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public string[] TryReadLines(string filePath)
        {
            if (File.Exists(filePath))
                return File.ReadAllLines(filePath);

            return null;
        }

        /// <summary>
        /// Extracts the exact file or directory name from the given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string ExtractName(string path)
        {
            if (File.Exists(path))
            {
                return Path.GetFileName(path);
            }
            else if (Directory.Exists(path))
            {
                return Path.GetDirectoryName(path);
            }
            
            return null;
        }

        /// <summary>
        /// Deletes file or directory by the given path, returns warning if it is not possible
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string Delete(string path)
        {
            if (File.Exists(path))
            { 
                File.Delete(path);
            }
            else if (Directory.Exists(path))
            {
                if (Directory.EnumerateFileSystemEntries(path).Any())
                    return "Can not delete directory - it is not empty";

                Directory.Delete(path);
            }

            return null;
        }

        /// <summary>
        /// Creates a directory by the given path, returns warning if it is not possible
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string MakeFolder(string path)
        {
            if (Directory.Exists(path))
                return "Duplicate folder name";
            
            Directory.CreateDirectory(path);

            return null;
        }

        /// <summary>
        /// Creates a file by the given path, if template path given copies all it's data to new file, returns warning if it is not possible
        /// </summary>
        /// <param name="path"></param>
        /// <param name="templatePath"></param>
        /// <returns></returns>
        public string MakeFile(string path, string templatePath)
        {
            if (File.Exists(path))
                return "Duplicate file name";

            try
            {
                if (templatePath.HasValue())
                { 
                    File.Copy(templatePath, path);
                }
                else
                {    
                    File.Create(path);
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return null;
        }

        /// <summary>
        /// Reads file by the given path, if file not exists creates and fills with default content
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="defaultContent"></param>
        /// <returns></returns>
        public string ReadOrCreate(string filePath, string defaultContent = "")
        {
            if (File.Exists(filePath))
                return File.ReadAllText(filePath);
            
            using (FileStream fs = File.Create(filePath))
            {
                var bytes = Encoding.UTF8.GetBytes(defaultContent);
                fs.Write(bytes, 0, bytes.Length);                
            }

            return defaultContent;
        }

        /// <summary>
        /// Combines a set of paths into one solid sequence
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public string Combine(params string[] paths) => Path.Combine(paths);

        /// <summary>
        /// Enumerates all filtered files in the given directory
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IEnumerable<string> EnumerateFiles(string directory, string filter = null)
        {
            if (Directory.Exists(directory))
                return Directory.EnumerateFiles(directory, filter);

            throw new ArgumentException($"Directory not found [{directory}]");
        }

        /// <summary>
        /// Tries to deserialize an object from the given file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T Deserialize<T>(string path)
            where T: class
        {
            var formatter = new BinaryFormatter();
            try
            {
                using (var fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    return formatter.Deserialize(fs) as T;
                }
            }
            catch (Exception ex)
            {
                IoCContainer.Resolve<ILogger>().PushError(ex, formatter, "Can not serialize data");
            }
            
            return null;
        }
    }
}