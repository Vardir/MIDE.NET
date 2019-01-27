using System;
using MIDE.Helpers;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace MIDE.FileSystem
{
    public abstract class FileManager
    {
        protected Dictionary<ApplicationPath, string> specialPaths;
        protected Dictionary<string, string> allPaths;

        public FileManager()
        {
            specialPaths = new Dictionary<ApplicationPath, string>();
            allPaths = new Dictionary<string, string>();
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
        public void LoadPaths(JArray array)
        {
            foreach (var obj in array)
            {
                if (obj.Type != JTokenType.Object)
                    throw new FormatException("Expected JSON object as paths item");
                var jo = obj as JObject;
                var key = jo.Properties().FirstWith(p => p.Name == "key", p => p.Value);
                if (key == null)
                    throw new FormatException("Expected 'key' property for path item");
                if (key.Type != JTokenType.String)
                    throw new FormatException("Expected JSON string as item's 'key' value");
                var value = jo.Properties().FirstWith(p => p.Name == "path", p => p.Value);
                if (value == null)
                    throw new FormatException("Expected 'value' property for path item");
                if (value.Type != JTokenType.String)
                    throw new FormatException("Expected JSON string as items's 'path' value");
                AddOrUpdate(key.ToString(), value.ToString());
            }
        }

        public string GetPath(ApplicationPath folder) => specialPaths[folder];
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
        
        public abstract string MapPath(string path);
        public abstract string TryRead(string filePath);
        public abstract string ReadOrCreate(string filePath, string defaultContent = "");
        public abstract IEnumerable<string> EnumerateFiles(string directory, string filter = null);
    }

    public enum ApplicationPath
    {
        UserSettings, DefaultForSolutions,
        AppAssets, Root, Installed, Themes, Extensions
    }
}