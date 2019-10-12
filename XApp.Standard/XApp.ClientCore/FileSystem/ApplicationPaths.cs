using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Vardirsoft.Shared.Helpers;

using Vardirsoft.XApp.IoC;
using Vardirsoft.XApp.API;
using Vardirsoft.XApp.Logging;
using Vardirsoft.XApp.Schemes.JSON;

namespace Vardirsoft.XApp.FileSystem
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
            paths = new Dictionary<string, string>
            {
                [LOGS] = "root\\",
                [ROOT] = "root\\logs\\",
                [TEMP] = "root\\temp\\",
                [TASKS] = "root\\tasks\\",
                [ASSETS] = "root\\assets\\",
                [THEMES] = "root\\themes\\",
                [PROJECTS] = "root\\projects\\",
                [TEMPLATES] = "root\\templates\\",
                [EXTENSIONS] = "root\\extensions\\",
                [SETTINGS] = "root\\user.settings.json"
            };
            
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
            {
                paths[pathId] = value;
            }
            else
            {
                paths.Add(pathId, value);
            }
        }

        /// <summary>
        /// Loads all application paths from the given file
        /// </summary>
        /// <param name="path"></param>
        public void LoadFrom(string path)
        {
            var fileManager = IoCContainer.Resolve<IFileManager>();

            if (fileManager.FileExists(path))
            {
                var fileData = fileManager.TryRead(path);
                if (fileData == null)
                    return;
                
                ApplicationPathItem[] pathItems = null;
                try
                {
                    pathItems = JsonConvert.DeserializeObject<ApplicationPathItem[]>(fileData);
                }
                catch (Exception ex)
                {
                    IoCContainer.Resolve<ILogger>().PushError(ex, this, "Couldn't load application paths: invalid syntax");
                }

                LoadFrom(pathItems);
            }
        }

        /// <summary>
        /// Loads all application paths from the given collection
        /// </summary>
        /// <param name="collection"></param>
        public void LoadFrom(IEnumerable<ApplicationPathItem> collection)
        {
            if (collection == null)
                return;

            var fileManager = IoCContainer.Resolve<IFileManager>();

            foreach (var path in collection)
            {
                if (!fileManager.Exists(path.Value))
                {
                    if (fileManager.IsFile(path.Value))
                    {
                        fileManager.MakeFile(path.Value, null);
                    }
                    else
                    {
                        fileManager.MakeFolder(path.Value);
                    }
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
            var folder = this[pathId];
            if (folder.HasValue())
                return IoCContainer.Resolve<IFileManager>().Combine(folder, file);            

            return null;
        }

        /// <summary>
        /// Gets the given path by key. If the path is not registered yet, creates new entry based on the given path
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string GetOrAddPath(string key, string defaultValue = null)
        {
            if (paths.ContainsKey(key))
                return paths[key];
            
            AddOrUpdate(key, defaultValue);
            
            return defaultValue;
        }
    }
}