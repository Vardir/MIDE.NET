using Vardirsoft.Shared.Helpers;

namespace Vardirsoft.XApp.FileSystem
{
    /// <summary>
    /// A record-type structure to represent any item in file system
    /// </summary>
    public struct DirectoryItem
    {
        public readonly string Name;
        public readonly string FullPath;
        public readonly FSObjectClass ItemClass;

        public DirectoryItem(string fullPath, FSObjectClass itemClass)
        {
            FullPath = fullPath;
            Name = itemClass.IsDrive ? fullPath : IOHelper.GetFileFolderName(fullPath);
            ItemClass = itemClass;
        }
    }
}