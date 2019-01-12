using System.Linq;
using MIDE.Helpers;
using MIDE.FileSystem;
using MIDE.API.Commands;
using System.Collections.Generic;
using MIDE.API.Measurements;

namespace MIDE.API.Components
{
    public class FileExplorer : Tab
    {
        private TreeView treeView;

        public FileExplorer(string id) : base(id)
        {
            IsSealed = true;
            InitializeComponents();
        }

        public void Show(string path)
        {
            treeView.Items.Clear();
            treeView.Items.AddRange(FileSystemInfo.GetDirectoryContents(path)
                   .Select(item => new FileExplorerItem(item.name, item.fullPath, item.itemClass)));
        }

        protected override void InitializeComponents()
        {
            StackPanel stack = new StackPanel("container");
            stack.Orientation = StackOrientation.Vertical;
            ActionTextBox searchBox = new ActionTextBox("search");
            searchBox.ActionButton.Caption = "@";
            searchBox.Margin = new BoundingBox(0, 0, 0, 5);
            treeView = new TreeView("files-view");

            ContentContainer = stack;
            stack.AddChild(searchBox);
            stack.AddChild(treeView);
        }        
    }

    public class FileExplorerItem : TreeViewItem
    {
        private bool isExpanded;
        private string fullPath;
        private FSObjectClass fsObjectClass;

        public override bool CanExpand => !ObjectClass.IsFile;
        public override bool IsExpanded
        {
            get => isExpanded;
            set
            {
                if (value)
                    Expand();
                isExpanded = value;
            }
        }
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

        public FileExplorerItem(string caption, string fullPath, FSObjectClass fsObjectClass) : base()
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