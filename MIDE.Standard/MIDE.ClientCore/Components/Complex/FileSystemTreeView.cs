using System;
using System.Linq;
using MIDE.Helpers;
using MIDE.Commands;
using MIDE.FileSystem;
using System.Collections.Generic;

namespace MIDE.Components.Complex
{
    public class FileSystemTreeView : TreeView
    {
        public string FileFilter { get; set; }
        public Func<DirectoryItem, TreeViewItem> Generator { get; set; }

        public FileSystemTreeView(string id) : base(id)
        {

        }
        
        public void Show(string path)
        {
            mItems.Clear();

            var items = Enumerable.Empty<DirectoryItem>();
            if (path == @"\")
            {
                items = FileSystemInfo.GetLogicalDrives();
            }
            else
            {
                items = FileSystemInfo.GetDirectoryContents(path, FileFilter);
            }

            mItems.AddRange(items.Select(Generator));
        }
    }

    public class FileSystemTreeViewItem : TreeViewItem
    {
        private string fullPath;
        private FSObjectClass fsObjectClass;

        public override bool CanExpand => !ObjectClass.IsFile;
        public FSObjectClass ObjectClass
        {
            get => fsObjectClass;
            set
            {
                if (fsObjectClass == value)
                    return;

                fsObjectClass = value;
                ItemClass = fsObjectClass.Id;
                ItemGlyph = fsObjectClass.ObjectGlyph;
                OnPropertyChanged();
            }
        }
        public string FullPath
        {
            get => fullPath;
            set
            {
                if (value == null || fullPath == value)
                    return;
                
                fullPath = value;
                OnPropertyChanged();
            }
        }

        public FileSystemTreeViewItem(DirectoryItem directoryItem)
            : this(directoryItem.name, directoryItem.fullPath, directoryItem.itemClass)
        {
            
        }
        public FileSystemTreeViewItem(string caption, string fullPath, FSObjectClass fsObjectClass) : base()
        {
            ExpandCommand = new RelayCommand(Expand, () => CanExpand);

            Caption = caption;
            FullPath = fullPath;
            ObjectClass = fsObjectClass;
            
            ClearChildren();
        }

        protected override TreeViewItem CloneInternal()
        {
            var clone = new FileSystemTreeViewItem(Caption, fullPath, fsObjectClass);
            return clone;
        }
        protected override IEnumerable<TreeViewItem> GetChildItems()
        {
            return FileSystemInfo.GetDirectoryContents(FullPath)
                   .Select(item => new FileExplorerItem(item));
        }

        protected override void OnChildrenCleared()
        {
            if (!ObjectClass.IsFile)
                Children.Add(null);
        }
    }

}
