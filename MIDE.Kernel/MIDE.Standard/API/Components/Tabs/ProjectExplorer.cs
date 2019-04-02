using System.Linq;
using MIDE.FileSystem;
using MIDE.API.Commands;
using System.Collections.Generic;

namespace MIDE.API.Components
{
    public class ProjectExplorer : Tab
    {
        public ProjectTree ProjectTree { get; }

        public ProjectExplorer(string id, ProjectTree tree, bool allowDuplicates = false)
            : base(id, allowDuplicates)
        {
            ProjectTree = tree;
        }
    }

    public abstract class ProjectTree : TreeView
    {
        public ProjectTree(string id) : base(id) { }
    }

    public class ProjectItem : TreeViewItem
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

        public ProjectItem(string caption, string fullPath, FSObjectClass fsObjectClass) : base()
        {
            ExpandCommand = new RelayCommand(Expand);

            Caption = caption;
            FullPath = fullPath;
            ObjectClass = fsObjectClass;

            ClearChildren();
        }

        protected override IEnumerable<TreeViewItem> GetChildItems()
        {
            return FileSystemInfo.GetDirectoryContents(FullPath)
                   .Select(item => new FileExplorerItem(item.name, item.fullPath, item.itemClass));
        }

        protected override void OnChildrenCleared()
        {
            if (!ObjectClass.IsFile)
                Children.Add(null);
        }
    }
}