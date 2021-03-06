﻿using MIDE.Helpers;

namespace MIDE.FileSystem
{
    /// <summary>
    /// A record-type structure to represent any item in file system
    /// </summary>
    public struct DirectoryItem
    {
        public readonly string name;
        public readonly string fullPath;
        public readonly FSObjectClass itemClass;

        public DirectoryItem(string fullPath, FSObjectClass itemClass)
        {
            this.fullPath = fullPath;
            name = itemClass.IsDrive ? fullPath : IOHelpers.GetFileFolderName(fullPath);
            this.itemClass = itemClass;
        }
    }
}