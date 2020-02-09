using System;
using System.Collections.Generic;
using System.Diagnostics;

using Newtonsoft.Json;

using Vardirsoft.Shared.Helpers;

using Vardirsoft.XApp.IoC;
using Vardirsoft.XApp.API;
using Vardirsoft.XApp.Helpers;
using Vardirsoft.XApp.Logging;
using Vardirsoft.XApp.Schemes.JSON;

namespace Vardirsoft.XApp.FileSystem
{
    public class ApplicationPaths
    {
        private static ApplicationPaths _instance;
        public static ApplicationPaths Instance { [DebuggerStepThrough] get => _instance ??= new ApplicationPaths(); }

        private readonly Dictionary<string, string> _paths;

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
            [DebuggerStepThrough] get => _paths.TryGetValue(pathId, out var path) ? path : null;
        }

        private ApplicationPaths()
        {
            _paths = new Dictionary<string, string>
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
            Guard.EnsureNonEmpty(pathId, typeof(ArgumentException), "Path Key can not be null or empty");
            Guard.EnsureNonEmpty(value, typeof(ArgumentException), "Path Value can not be null or empty");

            if (_paths.ContainsKey(pathId))
            {
                _paths[pathId] = value;
            }
            else
            {
                _paths.Add(pathId, value);
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
                if (fileData is null)
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
            if (collection is null)
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
            
            return folder.HasValue() ? IoCContainer.Resolve<IFileManager>().Combine(folder, file) : null;
        }

        /// <summary>
        /// Gets the given path by key. If the path is not registered yet, creates new entry based on the given path
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string GetOrAddPath(string key, string defaultValue = null)
        {
            if (_paths.ContainsKey(key))
                return _paths[key];
            
            AddOrUpdate(key, defaultValue);
            
            return defaultValue;
        }
    }
}