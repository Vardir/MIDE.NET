using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

using Vardirsoft.Shared.Helpers;

using Vardirsoft.XApp.IoC;
using Vardirsoft.XApp.API;
using Vardirsoft.XApp.Helpers;
using Vardirsoft.XApp.Logging;
using Vardirsoft.XApp.Visuals;
using Vardirsoft.XApp.Schemes.JSON;

namespace Vardirsoft.XApp.FileSystem
{
    public class FileSystemInfo
    {
        private Dictionary<string, FSObjectClass> fsObjectClasses;

        public const string FILE_EXTENSION_PATTERN = @"^\.[A-z0-9]+$";
        /// <summary>
        /// Special file system object class that is used to indicate any file
        /// </summary>
        public const string ANY_FILE_EXTENSION = "*";
        /// <summary>
        /// Special file system object class that is used to indicate a folder
        /// </summary>
        public const string FOLDER_EXTENSION = "@f";
        /// <summary>
        /// Special file system object class that is used to indicate a drive
        /// </summary>
        public const string DRIVE_EXTENSION = "@d";

        public IFilePropertiesExtractor FilePropertiesExtractor { get; set; }

        public FSObjectClass this[string id] { [DebuggerStepThrough] get => fsObjectClasses[id]; }
        
        public FileSystemInfo()
        {
            fsObjectClasses = new Dictionary<string, FSObjectClass> {
                ["drive"]  = new FSObjectClass("drive", DRIVE_EXTENSION),
                ["folder"] = new FSObjectClass("folder", FOLDER_EXTENSION),
                ["file"]   = new FSObjectClass("file", ANY_FILE_EXTENSION)
            };

            Initialize();
        }
        
        [DebuggerStepThrough]
        public bool IsRegistered(FSObjectClass fsoClass) => fsObjectClasses.ContainsKey(fsoClass.Id);
        
        public bool RegisterClass(FSObjectClass fsoClass)
        {
            Guard.EnsureNotNull(fsoClass, typeof(ArgumentNullException));
            Guard.EnsureNot(IsSpecialExtension(fsoClass.Extension), typeof(ArgumentException), "Can not add duplicate special file system class");

            if (fsObjectClasses.ContainsKey(fsoClass.Id))
                return false;

            fsObjectClasses.Add(fsoClass.Id, fsoClass);

            return true;
        }
        public FSObjectClass Find(string id)
        {
            if (fsObjectClasses.TryGetValue(id, out FSObjectClass fsoClass))
                return fsoClass;

            return null;
        }
        
        public FSObjectClass FindBy(string extension)
        {
            if (extension == ANY_FILE_EXTENSION)
                return fsObjectClasses["file"];

            if (extension == FOLDER_EXTENSION)
                return fsObjectClasses["folder"];

            if (extension == DRIVE_EXTENSION)
                return fsObjectClasses["drive"];

            foreach (var kvp in fsObjectClasses)
            {
                if (kvp.Value.Extension == extension)
                    return kvp.Value;
            }

            return fsObjectClasses["file"];
        }
        public IEnumerable<FSObjectClass> Select(Func<FSObjectClass, bool> match)
        {
            Guard.EnsureNotNull(match, typeof(ArgumentException));

            foreach (var kvp in fsObjectClasses)
            {
                if (match(kvp.Value))
                    yield return kvp.Value;
            }
        }
        /// <summary>
        /// Extracts properties of the file or directory by the given path. If path is invalid, throws an exception
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IEnumerable<(string prop, string val)> ExtractProperties(string path)
        {
            if (FilePropertiesExtractor is null)
            {
                var file = new FileInfo(path);
                if (file.Exists)
                    return GetFileProperties(file);                

                var directory = new DirectoryInfo(path);
                if (directory.Exists)
                    return GetDirectoryProperties(directory);

                throw new ArgumentException("The given path is invalid", nameof(path));
            }

            return FilePropertiesExtractor.ExtractProperties(path);
        }

        public static bool IsSpecialExtension(string extension)
        {
            return extension == FOLDER_EXTENSION || extension == ANY_FILE_EXTENSION || extension == DRIVE_EXTENSION;
        }
        public static bool IsValidExtension(string extension)
        {
            return IsSpecialExtension(extension) || Regex.IsMatch(extension, FILE_EXTENSION_PATTERN);
        }
        public static IEnumerable<DirectoryItem> GetLogicalDrives()
        {
            var fileSystemInfo = IoCContainer.Resolve<FileSystemInfo>();

            return Directory.GetLogicalDrives().Select(drive => new DirectoryItem(drive, fileSystemInfo["drive"]));
        }
        public static LinkedList<DirectoryItem> GetDirectoryContents(string fullPath, string searchPattern = null)
        {
            var items = new LinkedList<DirectoryItem>();
            var fileSystemInfo = IoCContainer.Resolve<FileSystemInfo>();

            if (IoCContainer.Resolve<IFileManager>().DirectoryExists(fullPath))
            {
                try
                {
                    var directories = Directory.GetDirectories(fullPath);
                    if (directories.HasItems())
                    {
                        items.AddRange(directories.Select(directory => new DirectoryItem(directory, fileSystemInfo["folder"])));
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    IoCContainer.Resolve<ILogger>().PushWarning(ex.Message);
                }

                try
                {
                    var files = searchPattern.HasValue() ? Directory.GetFiles(fullPath, searchPattern) : Directory.GetFiles(fullPath);
                    if (files.HasItems())
                    {
                        items.AddRange(files.Select(file =>
                        {
                            var extension = Path.GetExtension(file);

                            return new DirectoryItem(file, fileSystemInfo.FindBy(extension));
                        }));
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    IoCContainer.Resolve<ILogger>().PushWarning(ex.Message);
                }

                return items;
            }

            return items;
        }

        private void Initialize()
        {
            var fileManager = IoCContainer.Resolve<IFileManager>();
            var file = fileManager.Combine(ApplicationPaths.Instance[ApplicationPaths.ASSETS], "file-system-items.json");
            var fileData = fileManager.ReadOrCreate(file, "{ \"file-extensions\": null, \"file-editors\": null }");
            var parameters = JsonConvert.DeserializeObject<FileSystemItemParameters>(fileData);

            if (((ICollection)parameters.FileExtensions).HasItems())
            { 
                LoadFileExtensions(parameters);
            }

            if (((ICollection)parameters.FileEditors).HasItems())
            {    
                LoadFileEditors(parameters);
            }

            LoadItemIcons();
        }
        private void LoadItemIcons()
        {
            foreach (var kvp in fsObjectClasses)
            {
                kvp.Value.ObjectGlyph = IoCContainer.Resolve<AssetsManager>().GlyphPool[kvp.Key];
            }
        }
        private void LoadFileExtensions(FileSystemItemParameters parameters)
        {
            foreach (var kvp in parameters.FileExtensions)
            {
                Update(kvp.Key, kvp.Value, null, null);
            }
        }
        private void LoadFileEditors(FileSystemItemParameters parameters)
        {
            foreach (var kvp in parameters.FileEditors)
            {
                Update(kvp.Key, null, kvp.Value, null);
            }
        }
        private void Update(string key, string extension, string editor, Glyph glyph)
        {
            if (fsObjectClasses.TryGetValue(key, out FSObjectClass objectClass))
            {
                fsObjectClasses[key] = objectClass.With(extension, editor, glyph);
            }
            else
            {
                fsObjectClasses.Add(key, new FSObjectClass(key, extension, editor, glyph));
            }
        }

        private IEnumerable<(string prop, string val)> GetFileProperties(FileInfo file)
        {
            yield return ("Creation time UTC", file.CreationTimeUtc.ToString(CultureInfo.InvariantCulture));
            yield return ("Parent directory", file.DirectoryName);
            yield return ("Extension", file.Extension);
            yield return ("Length", file.Length.ToString());
        }
        private IEnumerable<(string prop, string val)> GetDirectoryProperties(DirectoryInfo directory)
        {
            yield return ("Creation time UTC", directory.CreationTimeUtc.ToString(CultureInfo.InvariantCulture));
            yield return ("Parent directory", directory.Parent?.FullName);
        }
    }
}