using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using Vardirsoft.Shared.MVVM;
using Vardirsoft.Shared.Helpers;

using Vardirsoft.XApp.API;
using Vardirsoft.XApp.Visuals;

namespace Vardirsoft.XApp.Components
{
    public class TreeView : LayoutComponent
    {
        private bool multiselect;
        private TreeViewItem selectedItem;
        
        protected ObservableCollection<TreeViewItem> mItems;

        public bool Multiselect
        {
            get => multiselect;
            set => SetWithNotify(ref multiselect, value);
        }
        public TreeViewItem SelectedItem
        {
            get => selectedItem;
            set => SetWithNotify(ref selectedItem, value, true);
        }
        public List<TreeViewItem> SelectedItems { get; }
        public ReadOnlyObservableCollection<TreeViewItem> Items { get; }

        public TreeView(string id) : base(id)
        {
            SelectedItems = new List<TreeViewItem>();
            mItems = new ObservableCollection<TreeViewItem>();
            mItems.CollectionChanged += Items_CollectionChanged;
            Items = new ReadOnlyObservableCollection<TreeViewItem>(mItems);
        }

        public void Clear()
        {
            mItems.Clear();
            SelectedItem = null;
            SelectedItems.Clear();
        }

        protected override LayoutComponent CloneInternal(string id)
        {
            var clone = new TreeView(id);
            clone.Items.AddRange(Items.Select(item => item.Clone()));

            return clone;
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    OnItemsAdded(e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    OnItemsRemove(e.OldItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    OnItemsReplaced(e.OldItems, e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    OnItemsRemove(e.OldItems);
                    break;
            }
        }
        private void OnItemsAdded(IList list)
        {
            if (list == null)
                return;

            foreach (var item in list)
            {
                var tvi = item as TreeViewItem;

                tvi.ParentTree = this;
                tvi.PropertyChanged += OnItemPropertyChanged;
            }
        }
        private void OnItemsRemove(IList list)
        {
            if (list == null)
                return;

            foreach (var item in list)
            {
                var tvi = item as TreeViewItem;

                tvi.ParentTree = null;
                tvi.PropertyChanged -= OnItemPropertyChanged;
            }
        }
        private void OnItemsReplaced(IList old, IList newItems)
        {
            OnItemsRemove(old);
            OnItemsAdded(newItems);
        }

        internal void OnItemPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var item = sender as TreeViewItem;

            if (item != null && item.ParentTree == this)
            {
                if (args.PropertyName == nameof(TreeViewItem.IsSelected))
                {
                    SelectedItem = item;
                    SelectedItems.Clear();

                    if (!SelectedItems.Contains(item))
                    {    
                        SelectedItems.Add(item);
                    }
                }
            }
        }
    }

    public abstract class TreeViewItem : BaseViewModel, ICloneable<TreeViewItem>
    {
        private bool isExpanded;
        private bool isSelected;
        private string caption;
        private string itemClass;
        private Glyph glyph;
        private ContextMenu contextMenu;

        public abstract bool CanExpand { get; }
        public virtual bool IsExpanded
        {
            get => isExpanded;
            set
            {
                if (value)
                {     
                   Expand();
                }

                isExpanded = value;
            }
        }
        public bool IsSelected
        {
            get => isSelected;
            set => SetWithNotify(ref isSelected, value);
        }
        /// <summary>
        /// Type of an item (e.g. 'drive', 'file' and 'directory' for file system objects)
        /// </summary>
        public string Caption
        {
            get => caption;
            set => SetWithNotify(ref caption, value, true);
        }
        public string ItemClass
        {
            get => itemClass;
            set => SetWithNotify(ref itemClass, value, true);
        }
        public Glyph ItemGlyph
        {
            get => glyph;
            set => SetWithNotify(ref glyph, value, true);
        }
        public TreeView ParentTree { get; set; }
        public TreeViewItem Parent { get; set; }
        public ContextMenu ContextMenu
        {
            get => contextMenu;
            set => SetWithNotify(ref contextMenu, value, true);
        }
        public ObservableCollection<TreeViewItem> Children { get; }

        public BaseCommand ExpandCommand { get; protected set; }

        public TreeViewItem()
        {
            Children = new ObservableCollection<TreeViewItem>();
        }

        public override string ToString() => $"tree-view-item :: {Caption} [{ItemClass}]";

        public TreeViewItem Clone()
        {
            var clone = CloneInternal();
            clone.Children.AddRange(Children.Select(item => item.Clone()));
            clone.caption = caption;
            clone.itemClass = itemClass;
            clone.glyph = glyph;
            clone.contextMenu = contextMenu;

            return clone;
        }
        public TreeViewItem Clone(string _) => Clone();

        protected virtual void OnChildrenClearing()
        {
            if (Children.Count != 0 && Children[0] != null)
            {   
                foreach (var child in Children)
                {
                    child.ParentTree = null;
                    child.PropertyChanged -= ParentTree.OnItemPropertyChanged;
                    child.ClearChildren();
                }
            }
        }
        protected virtual void OnChildrenCleared() { }

        protected void ClearChildren()
        {
            OnChildrenClearing();
            Children.Clear();
            OnChildrenCleared();
        }
        protected void Expand()
        {
            if (CanExpand)
            {
                //if (Children.Count > 0 && Children[0] != null)
                //    return;

                var childItems = GetChildItems();
                ClearChildren();
                Children.Clear();
                Children.AddRange(childItems);

                foreach (var child in Children)
                {
                    child.ParentTree = ParentTree;
                    child.PropertyChanged += ParentTree.OnItemPropertyChanged;
                    child.Parent = this;
                }
            }
        }

        protected abstract TreeViewItem CloneInternal();
        protected abstract IEnumerable<TreeViewItem> GetChildItems();
    }
}