﻿using System;
using System.Linq;
using MIDE.Helpers;
using MIDE.FileSystem;
using MIDE.API.Commands;
using System.Collections.Generic;

namespace MIDE.API.Components.Complex
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
            Items.Clear();
            var items = Enumerable.Empty<DirectoryItem>();
            if (path == @"\")
                items = FileSystemInfo.GetLogicalDrives();
            else
                items = FileSystemInfo.GetDirectoryContents(path, FileFilter);

            Items.AddRange(items.Select(Generator));
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
                OnPropertyChanged(nameof(ObjectClass));
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
                OnPropertyChanged(nameof(FullPath));
            }
        }

        public FileSystemTreeViewItem(DirectoryItem directoryItem)
            : this(directoryItem.name, directoryItem.fullPath, directoryItem.itemClass)
        {
            
        }
        public FileSystemTreeViewItem(string caption, string fullPath, FSObjectClass fsObjectClass) : base()
        {
            ExpandCommand = new RelayCommand(Expand);

            Caption = caption;
            FullPath = fullPath;
            ObjectClass = fsObjectClass;
            
            ClearChildren();
        }

        protected override TreeViewItem CloneInternal()
        {
            FileSystemTreeViewItem clone = new FileSystemTreeViewItem(Caption, fullPath, fsObjectClass);
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
