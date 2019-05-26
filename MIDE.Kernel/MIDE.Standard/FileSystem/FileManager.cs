using System;
using System.IO;
using System.Text;
using System.Linq;
using MIDE.Helpers;
using MIDE.Application;
using MIDE.Schemes.JSON;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace MIDE.FileSystem
{
    public abstract class FileManager
    {
        protected Dictionary<ApplicationPath, string> specialPaths;
        protected Dictionary<string, string> allPaths;

        public static FileManager Instance { get; set; }

        public string this[ApplicationPath applicationPath]
        {
            get
            {
                if (specialPaths.TryGetValue(applicationPath, out string path))
                    return path;
                return null;
            }
        }

        public FileManager()
        {
            allPaths = new Dictionary<string, string>();
            specialPaths = new Dictionary<ApplicationPath, string>();
        }

        /// <summary>
        /// Recursively deletes a directory from a specified path
        /// </summary>
        /// <param name="path"></param>
        public void DeleteDirectory(string path)
        {
            if (!Directory.Exists(path))
                return;
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                File.Delete(file.FullName);
            }
            foreach (DirectoryInfo subdir in dir.GetDirectories())
            {
                DeleteDirectory(subdir.FullName);
            }
            Directory.Delete(path);
        }
        /// <summary>
        /// Adds the given known path entry or updates existing one
        /// </summary>
        /// <param name="path"></param>
        /// <param name="value"></param>
        public void AddOrUpdate(ApplicationPath path, string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Path Value can not be null or empty", nameof(value));

            if (specialPaths.ContainsKey(path))
            {
                specialPaths[path] = value;
                allPaths[path.ToString().ToLower()] = value;
            }
            else
            {
                specialPaths.Add(path, value);
                allPaths.Add(path.ToString().ToLower(), value);
            }
        }
        /// <summary>
        /// Adds the given path entry or updates existing one
        /// </summary>
        /// <param name="pathKey"></param>
        /// <param name="value"></param>
        public void AddOrUpdate(string pathKey, string value)
        {
            if (string.IsNullOrEmpty(pathKey))
                throw new ArgumentException("Path Key can not be null or empty", nameof(pathKey));
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Path Value can not be null or empty", nameof(value));

            if (Enum.TryParse(pathKey, true, out ApplicationPath appPath))
            {
                AddOrUpdate(appPath, value);
                return;
            }
            if (allPaths.ContainsKey(pathKey))
                allPaths[pathKey] = value;
            else
                allPaths.Add(pathKey, value);
        }
        /// <summary>
        /// Loads all application paths from the given collection
        /// </summary>
        /// <param name="collection"></param>
        public void LoadPaths(IEnumerable<ApplicationPathItem> collection)
        {
            if (collection == null)
                return;

            foreach (var path in collection)
            {
                AddOrUpdate(path.Key, path.Value);
            }
        }

        /// <summary>
        /// Writes the given string to the given file
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        public virtual void Write(string data, string path) => File.WriteAllText(path, data);
        /// <summary>
        /// Writes the given lines of text to the given file
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        public virtual void Write(string[] data, string path) => File.WriteAllLines(path, data);
        /// <summary>
        /// Copies all the files and subdirectories from the source directory to the given destination
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public virtual void Copy(string source, string destination)
        {
            if (!Directory.Exists(source))
                return;
            DirectoryInfo dir = new DirectoryInfo(source);
            DirectoryInfo[] dirs = dir.GetDirectories();
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }
            
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destination, file.Name);
                file.CopyTo(temppath, false);
            }
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destination, subdir.Name);
                Copy(subdir.FullName, temppath);
            }
        }
        /// <summary>
        /// Tries to serialize the given object in file, if it is impossible writes log
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        public virtual void Serialize(object data, string path)
        {
            if (data == null)
                File.WriteAllText(path, "<null>");
            var type = data.GetType();
            if (!type.HasAttribute<SerializableAttribute>())
                File.WriteAllText(path, data.ToString());

            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, data);
                }
            }
            catch (Exception ex)
            {
                AppKernel.Instance.AppLogger.PushError(ex, this, "Can not serialize data");
            }
        }
        /// <summary>
        /// Cleans all the contents of the given directory
        /// </summary>
        /// <param name="path"></param>
        public virtual void CleanDirectory(string path)
        {
            if (!IsDirectory(path))
                return;
            DirectoryInfo dir = new DirectoryInfo(path);
            DirectoryInfo[] dirs = dir.GetDirectories();

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                Delete(file.FullName);
            }
            foreach (DirectoryInfo subdir in dirs)
            {
                DeleteDirectory(subdir.FullName);
            }
        }

        /// <summary>
        /// Searches for the given known path entry
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public string GetPath(ApplicationPath folder)
        {
            if (specialPaths.TryGetValue(folder, out string path))
                return path;
            string folderPath = "";
            switch (folder)
            {
                case ApplicationPath.DefaultForProjects:
                    folderPath = "root\\projects\\"; break;
                case ApplicationPath.UserSettings:
                    folderPath = "root\\settings\\"; break;
                case ApplicationPath.Extensions:
                    folderPath = "root\\extensions\\"; break;
                case ApplicationPath.Templates:
                    folderPath = "root\\templates\\"; break;
                case ApplicationPath.AppAssets:
                    folderPath = "root\\assets\\"; break;
                case ApplicationPath.Themes:
                    folderPath = "root\\themes\\"; break;
                case ApplicationPath.Tasks:
                    folderPath = "root\\tasks\\"; break;
                case ApplicationPath.Logs:
                    folderPath = "root\\logs\\"; break;
                case ApplicationPath.Root:
                    folderPath = "root\\"; break;
                case ApplicationPath.Installed:
                    folderPath = ""; break;
            }
            return GetOrAddPath(folder, folderPath);
        }
        /// <summary>
        /// Combines file path with application path that contains the file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public string GetPath(ApplicationPath path, string file)
        {
            string folder = GetPath(path);
            return Combine(folder, file);
        }
        /// <summary>
        /// Gets the given application path. If the path is not registered yet, creates new entry based on the given path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string GetOrAddPath(ApplicationPath path, string defaultValue = null)
        {
            if (!specialPaths.ContainsKey(path))
            {
                AddOrUpdate(path, defaultValue);
                return defaultValue;
            }
            return specialPaths[path];
        }
        /// <summary>
        /// Searches for path by the given key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetPath(string key) => allPaths[key];
        /// <summary>
        /// Gets the given path by key. If the path is not registered yet, creates new entry based on the given path
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string GetOrAddPath(string key, string defaultValue = null)
        {
            if (!allPaths.ContainsKey(key))
            {
                AddOrUpdate(key, defaultValue);
                return defaultValue;
            }
            return allPaths[key];
        }
        public string GetAbsolutePath(string path) => Path.GetFullPath(path);

        /// <summary>
        /// Verifies if the given path exists, it can be either file or directory
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public virtual bool Exists(string path) => File.Exists(path) || Directory.Exists(path);
        /// <summary>
        /// Verifies if the given file exists
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public virtual bool IsFile(string path) => File.Exists(path);
        /// <summary>
        /// Verifies if the given directory exists
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public virtual bool IsDirectory(string path) => Directory.Exists(path);
        /// <summary>
        /// Maps the given path to application installation path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract string MapPath(string path);
        /// <summary>
        /// Tries to load bytes from the given file and returns null if file does not exist
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public virtual byte[] TryReadBytes(string path)
        {
            if (!File.Exists(path))
                return null;
            return File.ReadAllBytes(path);
        }
        public virtual string GetFilePath(string file)
        {
            if (!File.Exists(file))
                return null;
            return Path.GetDirectoryName(file);
        }
        /// <summary>
        /// Tries to load data from the given file and returns null if file does not exist
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public virtual string TryRead(string filePath)
        {
            if (!File.Exists(filePath))
                return null;
            return File.ReadAllText(filePath);
        }
        /// <summary>
        /// Extracts the exact file or directory name from the given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public virtual string ExtractName(string path)
        {
            if (File.Exists(path))
                return Path.GetFileName(path);
            else if (Directory.Exists(path))
                return Path.GetDirectoryName(path);
            return null;
        }
        /// <summary>
        /// Deletes file or directory by the given path, returns warning if it is not possible
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public virtual string Delete(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
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
        public virtual string MakeFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                return null;
            }
            else
                return "Duplicate folder name";
        }
        /// <summary>
        /// Creates a file by the given path, if template path given copies all it's data to new file, returns warning if it is not possible
        /// </summary>
        /// <param name="path"></param>
        /// <param name="templatePath"></param>
        /// <returns></returns>
        public virtual string MakeFile(string path, string templatePath)
        {
            if (File.Exists(path))
                return "Duplicate file name";
            try
            {
                if (templatePath != null)
                    File.Copy(templatePath, path);
                else
                    File.Create(path);
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
        public virtual string ReadOrCreate(string filePath, string defaultContent = "")
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
        /// <summary>
        /// Combines a set of paths into one solid sequence
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public virtual string Combine(params string[] paths) => Path.Combine(paths);
        /// <summary>
        /// Enumerates all filtered files in the given directory
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public virtual IEnumerable<string> EnumerateFiles(string directory, string filter = null)
        {
            if (!Directory.Exists(directory))
                throw new ArgumentException($"Directory not found [{directory}]");
            return Directory.EnumerateFiles(directory, filter);
        }
        /// <summary>
        /// Reads properties of the given file or directory
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract IEnumerable<(string prop, string val)> ExtractProperties(string path);
        /// <summary>
        /// Tries to deserialize an object from the given file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public virtual T Deserialize<T>(string path)
            where T: class
        {
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    return formatter.Deserialize(fs) as T;
                }
            }
            catch (Exception ex)
            {
                AppKernel.Instance.AppLogger.PushError(ex, this, "Can not serialize data");
            }
            return null;
        }
    }

    public enum ApplicationPath
    {
        Root,
        Logs,
        Tasks,
        Themes,
        AppAssets,
        Installed,
        Templates,
        Extensions,
        UserSettings,
        DefaultForProjects
    }
}