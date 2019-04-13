using System.Linq;
using MIDE.Application;
using MIDE.API.Commands;
using MIDE.API.DataModels;
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
            AddChild(tree);
        }

        public void LoadProject(Project project)
        {
            ProjectTree.Clear();
        }
        public void AttachProject(Project project)
        {

        }

        protected override Tab Create(string id, Toolbar toolbar, bool allowDuplicates)
        {
            ProjectExplorer clone = new ProjectExplorer(id, ProjectTree.Clone() as ProjectTree, allowDuplicates);

            return clone;
        }
    }

    public abstract class ProjectTree : TreeView
    {
        public ProjectTree(string id) : base(id) { }
    }

    public class ProjectItem : TreeViewItem
    {
        private string fullPath;
        private ProjectObjectClass pjObjectClass;

        public override bool CanExpand => ObjectClass.IsFolder;
        public ProjectObjectClass ObjectClass
        {
            get => pjObjectClass;
            set
            {
                if (pjObjectClass == value)
                    return;
                pjObjectClass = value;
                ItemClass = pjObjectClass.Id;
                ItemGlyph = pjObjectClass.ObjectGlyph;
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

        public ProjectItem(string caption, string fullPath, ProjectObjectClass pjObjectClass) : base()
        {
            ExpandCommand = new RelayCommand(Expand);

            Caption = caption;
            FullPath = fullPath;
            ObjectClass = pjObjectClass;

            ClearChildren();
        }

        protected override TreeViewItem CloneInternal()
        {
            ProjectItem clone = new ProjectItem(Caption, fullPath, pjObjectClass);
            return clone;
        }
        protected override IEnumerable<TreeViewItem> GetChildItems()
        {
            return null;
            //return FileSystemInfo.GetDirectoryContents(FullPath)
            //       .Select(item => new FileExplorerItem(item.name, item.fullPath, item.itemClass));
        }

        protected override void OnChildrenCleared()
        {
            if (!ObjectClass.IsFile)
                Children.Add(null);
        }
    }
}