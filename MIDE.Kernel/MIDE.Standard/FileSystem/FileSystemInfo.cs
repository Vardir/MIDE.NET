using System;
using System.IO;
using System.Linq;
using MIDE.Helpers;
using System.Drawing;
using MIDE.API.Visuals;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MIDE.FileSystem
{
    public class FileSystemInfo
    {
        private static FileSystemInfo instance;
        public static FileSystemInfo Instance => instance ?? (instance = new FileSystemInfo());

        private Dictionary<string, FSObjectClass> fsObjectClasses;

        public const string FILE_EXTENSION_PATTERN = @"^\.[A-z0-9]+$";
        /// <summary>
        /// Special file system object class that is used to indicate any file
        /// </summary>
        public const string ANY_FILE_EXTENSION = "*";
        /// <summary>
        /// Special file system object class that is used to indicate a folder
        /// </summary>
        public const string FOLDER_EXTENSION = "f";
        /// <summary>
        /// Special file system object class that is used to indicate a drive
        /// </summary>
        public const string DRIVE_EXTENSION = "d";

        public FSObjectClass this[string id] => fsObjectClasses[id];

        private FileSystemInfo ()
        {
            fsObjectClasses = new Dictionary<string, FSObjectClass>()
            {
                ["drive"] = new FSObjectClass("drive", DRIVE_EXTENSION, new Glyph("\uf0a0") { AlternateColor = Color.Orange }),
                ["folder"] = new FSObjectClass("folder", FOLDER_EXTENSION, new Glyph("\uf07b") { AlternateColor = Color.Orange }),
                ["file"] = new FSObjectClass("file", ANY_FILE_EXTENSION, new Glyph("\uf15b") { AlternateColor = Color.Silver })
            };
        }
        
        public bool IsRegistered(FSObjectClass fsoClass) => fsObjectClasses.ContainsKey(fsoClass.Id);
        public bool RegisterClass(FSObjectClass fsoClass)
        {
            if (fsoClass == null)
                throw new ArgumentNullException(nameof(fsoClass));
            if (IsSpecialExtension(fsoClass.Extension))
                throw new ArgumentException("Can not add duplicate special file system class", nameof(fsoClass));

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
                if (kvp.Key == extension)
                    return kvp.Value;
            }
            return fsObjectClasses["file"];
        }
        public IEnumerable<FSObjectClass> Select(Func<FSObjectClass, bool> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));
            foreach (var kvp in fsObjectClasses)
            {
                if (match(kvp.Value))
                    yield return kvp.Value;
            }
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
            return Directory.GetLogicalDrives().Select(drive => new DirectoryItem(drive, Instance["drive"]));
        }
        public static LinkedList<DirectoryItem> GetDirectoryContents(string fullPath)
        {
            var items = new LinkedList<DirectoryItem>();
            try
            {
                var directories = Directory.GetDirectories(fullPath);
                if (directories != null && directories.Length > 0)
                {
                    items.AddRange(directories.Select(directory => new DirectoryItem(directory, Instance["folder"])));
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                var files = Directory.GetFiles(fullPath);
                if (files != null && files.Length > 0)
                {
                    items.AddRange(files.Select(file =>
                    {
                        string extension = Path.GetExtension(file);
                        return new DirectoryItem(file, Instance.FindBy(extension));
                    }));
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return items;
        }
    }
}