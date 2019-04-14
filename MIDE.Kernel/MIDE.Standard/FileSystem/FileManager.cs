using System;
using MIDE.Schemes.JSON;
using System.Collections.Generic;

namespace MIDE.FileSystem
{
    public abstract class FileManager
    {
        protected Dictionary<ApplicationPath, string> specialPaths;
        protected Dictionary<string, string> allPaths;

        public FileManager()
        {
            allPaths = new Dictionary<string, string>();
            specialPaths = new Dictionary<ApplicationPath, string>();
        }

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
        public void LoadPaths(IEnumerable<ApplicationPathItem> collection)
        {
            if (collection == null)
                return;

            foreach (var path in collection)
            {
                AddOrUpdate(path.Key, path.Value);
            }
        }

        public abstract void Write(string data, string path);
        public abstract void Write(string[] data, string path);
        public abstract void Serialize(object data, string path);

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
                case ApplicationPath.Logs:
                    folderPath = "root\\logs\\"; break;
                case ApplicationPath.Root:
                    folderPath = "root\\"; break;
                case ApplicationPath.Installed:
                    folderPath = ""; break;
            }
            return GetOrAddPath(folder, folderPath);
        }
        public string GetPath(ApplicationPath path, string file)
        {
            string folder = GetPath(path);
            return Combine(folder, file);
        }
        public string GetOrAddPath(ApplicationPath path, string defaultValue = null)
        {
            if (!specialPaths.ContainsKey(path))
            {
                AddOrUpdate(path, defaultValue);
                return defaultValue;
            }
            return specialPaths[path];
        }
        public string GetPath(string key) => allPaths[key];
        public string GetOrAddPath(string key, string defaultValue = null)
        {
            if (!allPaths.ContainsKey(key))
            {
                AddOrUpdate(key, defaultValue);
                return defaultValue;
            }
            return allPaths[key];
        }

        public abstract bool Exists(string path);
        public abstract string MapPath(string path);
        public abstract string TryRead(string filePath);
        public abstract string ExtractName(string path);
        public abstract string Delete(string path);
        public abstract string MakeFolder(string path);
        public abstract string MakeFile(string path, string templatePath);
        public abstract string ReadOrCreate(string filePath, string defaultContent = "");
        public abstract string Combine(params string[] paths);
        public abstract IEnumerable<string> EnumerateFiles(string directory, string filter = null);
        public abstract IEnumerable<(string prop, string val)> ExtractProperties(string path);
    }

    public enum ApplicationPath
    {
        Root,
        Logs,
        Themes,
        AppAssets,
        Installed,
        Templates,
        Extensions,
        UserSettings,
        DefaultForProjects
    }
}