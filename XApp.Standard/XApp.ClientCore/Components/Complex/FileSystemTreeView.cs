using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

using Vardirsoft.Shared.MVVM;
using Vardirsoft.Shared.Helpers;

using Vardirsoft.XApp.FileSystem;

namespace Vardirsoft.XApp.Components.Complex
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
            _items.Clear();

            var items = Enumerable.Empty<DirectoryItem>();
            if (path == @"\")
            {
                items = FileSystemInfo.GetLogicalDrives();
            }
            else
            {
                items = FileSystemInfo.GetDirectoryContents(path, FileFilter);
            }

            _items.AddRange(items.Select(Generator));
        }
    }

    public class FileSystemTreeViewItem : TreeViewItem
    {
        private string _fullPath;
        private FSObjectClass _fsObjectClass;

        public override bool CanExpand { [DebuggerStepThrough] get => !ObjectClass.IsFile; }
        
        public FSObjectClass ObjectClass
        {
            [DebuggerStepThrough]
            get => _fsObjectClass;
            set
            {
                if (_fsObjectClass == value)
                    return;

                _fsObjectClass = value;
                ItemClass = _fsObjectClass.Id;
                ItemGlyph = _fsObjectClass.ObjectGlyph;
                
                NotifyPropertyChanged();
            }
        }
        public string FullPath
        {
            [DebuggerStepThrough]
            get => _fullPath;
            
            [DebuggerStepThrough]
            set => SetWithNotify(ref _fullPath, value, true);
        }

        public FileSystemTreeViewItem(DirectoryItem directoryItem)
            : this(directoryItem.Name, directoryItem.FullPath, directoryItem.ItemClass)
        {
            
        }
        public FileSystemTreeViewItem(string caption, string fullPath, FSObjectClass fsObjectClass)
        {
            ExpandCommand = new RelayCommand(Expand, () => CanExpand);

            Caption = caption;
            FullPath = fullPath;
            ObjectClass = fsObjectClass;
            
            ClearChildren();
        }

        protected override TreeViewItem CloneInternal()
        {
            var clone = new FileSystemTreeViewItem(Caption, _fullPath, _fsObjectClass);
            
            return clone;
        }
        protected override IEnumerable<TreeViewItem> GetChildItems()
        {
            // return FileSystemInfo.GetDirectoryContents(FullPath)
            //        .Select(item => new FileExplorerItem(item));
            yield break;
        }

        protected override void OnChildrenCleared()
        {
            if (!ObjectClass.IsFile)
                Children.Add(null);
        }
    }
}