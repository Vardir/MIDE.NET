using System;
using Newtonsoft.Json;
using MIDE.Schemes.JSON;
using System.Collections.Generic;

namespace MIDE.FileSystem
{
    public class ApplicationPaths
    {
        private static ApplicationPaths instance;
        public static ApplicationPaths Instance => instance ?? (instance = new ApplicationPaths());

        private readonly Dictionary<string, string> paths;

        #region Common Paths
        public const string LOGS = "logs";
        public const string ROOT = "root";
        public const string TEMP = "temp";
        public const string TASKS = "tasks";
        public const string ASSETS = "assets";
        public const string THEMES = "themes";
        public const string PROJECTS = "projects";
        public const string SETTINGS = "settings";
        public const string TEMPLATES = "templates";
        public const string EXTENSIONS = "extensions";
        #endregion

        public string this[string pathId]
        {
            get
            {
                if (paths.TryGetValue(pathId, out string path))
                    return path;
                return null;
            }
        }

        private ApplicationPaths()
        {
            paths = new Dictionary<string, string>();
            AddOrUpdate(LOGS, "root\\");
            AddOrUpdate(ROOT, "root\\logs\\");
            AddOrUpdate(TEMP, "root\\temp\\");
            AddOrUpdate(TASKS, "root\\tasks\\");
            AddOrUpdate(ASSETS, "root\\assets\\");
            AddOrUpdate(THEMES, "root\\themes\\");
            AddOrUpdate(PROJECTS, "root\\projects\\");
            AddOrUpdate(TEMPLATES, "root\\templates\\");
            AddOrUpdate(EXTENSIONS, "root\\extensions\\");
            AddOrUpdate(SETTINGS, "root\\user.settings.json");
            LoadFrom("paths.json");
        }

        /// <summary>
        /// Adds the given path entry or updates existing one
        /// </summary>
        /// <param name="pathId"></param>
        /// <param name="value"></param>
        public void AddOrUpdate(string pathId, string value)
        {
            if (string.IsNullOrEmpty(pathId))
                throw new ArgumentException("Path Key can not be null or empty", nameof(pathId));
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Path Value can not be null or empty", nameof(value));

            if (paths.ContainsKey(pathId))
                paths[pathId] = value;
            else
                paths.Add(pathId, value);
        }
        /// <summary>
        /// Loads all application paths from the given file
        /// </summary>
        /// <param name="path"></param>
        public void LoadFrom(string path)
        {
            if (!FileManager.FileExists(path))
                return;
            string fileData = FileManager.TryRead(path);
            if (fileData == null)
                return;
            ApplicationPathItem[] pathItems = null;
            try
            {
                pathItems = JsonConvert.DeserializeObject<ApplicationPathItem[]>(fileData);
            }
            catch (Exception ex)
            {

            }
            LoadFrom(pathItems);
        }
        /// <summary>
        /// Loads all application paths from the given collection
        /// </summary>
        /// <param name="collection"></param>
        public void LoadFrom(IEnumerable<ApplicationPathItem> collection)
        {
            if (collection == null)
                return;

            foreach (var path in collection)
            {
                if (!FileManager.Exists(path.Value))
                {
                    if (FileManager.IsFile(path.Value))
                        FileManager.MakeFile(path.Value, null);
                    else
                        FileManager.MakeFolder(path.Value);
                }
                AddOrUpdate(path.Key, path.Value);
            }
        }

        /// <summary>
        /// Combines file path with known application path that contains the file
        /// </summary>
        /// <param name="pathId"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public string GetFilePath(string pathId, string file)
        {
            string folder = this[pathId];
            if (folder == null)
                return null;
            return FileManager.Combine(folder, file);
        }
        /// <summary>
        /// Gets the given path by key. If the path is not registered yet, creates new entry based on the given path
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string GetOrAddPath(string key, string defaultValue = null)
        {
            if (!paths.ContainsKey(key))
            {
                AddOrUpdate(key, defaultValue);
                return defaultValue;
            }
            return paths[key];
        }
    }
}