using MIDE.Helpers;
using MIDE.API.Visuals;
using MIDE.API.Commands;
using MIDE.API.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MIDE.API.Components
{
    public class TreeView : LayoutComponent
    {
        public ObservableCollection<TreeViewItem> Items { get; }

        public TreeView(string id) : base(id)
        {
            Items = new ObservableCollection<TreeViewItem>();
        }
    }

    public abstract class TreeViewItem : BaseViewModel
    {
        private string caption;
        private string itemClass;
        private Glyph glyph;
        private ContextMenu contextMenu;

        public abstract bool CanExpand { get; }
        public abstract bool IsExpanded
        {
            get;
            set;
        }
        /// <summary>
        /// Type of an item (e.g. 'drive', 'file' and 'directory' for file system objects)
        /// </summary>
        public string Caption
        {
            get => caption;
            set
            {
                if (value == caption)
                    return;
                caption = value;
                OnPropertyChanged(nameof(Caption));
            }
        }
        public string ItemClass
        {
            get => itemClass;
            set
            {
                if (value == itemClass)
                    return;
                itemClass = value;
                OnPropertyChanged(nameof(ItemClass));
            }
        }
        public Glyph ItemGlyph
        {
            get => glyph;
            set
            {
                if (value == glyph)
                    return;
                glyph = value;
                OnPropertyChanged(nameof(ItemGlyph));
            }
        }
        public ContextMenu ContextMenu
        {
            get => contextMenu;
            set
            {
                if (value == contextMenu)
                    return;
                contextMenu = value;
                OnPropertyChanged(nameof(ContextMenu));
            }
        }
        public ObservableCollection<TreeViewItem> Children { get; }

        public ICommand ExpandCommand { get; protected set; }

        public TreeViewItem()
        {
            Children = new ObservableCollection<TreeViewItem>();
        }

        public override string ToString() => $"tree-view-item :: {Caption} [{ItemClass}]";

        protected virtual void OnChildrenCleared() { }

        protected void ClearChildren()
        {
            Children.Clear();
            OnChildrenCleared();
        }
        protected void Expand()
        {
            if (!CanExpand)
                return;
            if (Children.Count > 0 && Children[0] != null)
                return;

            var childItems = GetChildItems();
            Children.Clear();
            Children.AddRange(childItems);
        }

        protected abstract IEnumerable<TreeViewItem> GetChildItems();
    }
}