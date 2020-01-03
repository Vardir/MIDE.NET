using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;

using Vardirsoft.Shared.MVVM;
using Vardirsoft.Shared.Helpers;

using Vardirsoft.XApp.API;
using Vardirsoft.XApp.Visuals;

namespace Vardirsoft.XApp.Components
{
    [Obsolete]
    public class TreeView : LayoutComponent
    {
        private bool _multiselect;
        private TreeViewItem _selectedItem;
        
        protected readonly ObservableCollection<TreeViewItem> _items;

        public bool Multiselect
        {
            [DebuggerStepThrough]
            get => _multiselect;
            
            [DebuggerStepThrough]
            set => SetWithNotify(ref _multiselect, value);
        }
        
        public TreeViewItem SelectedItem
        {
            [DebuggerStepThrough]
            get => _selectedItem;
            
            [DebuggerStepThrough]
            set => SetWithNotify(ref _selectedItem, value, true);
        }
        
        public List<TreeViewItem> SelectedItems { get; }
        
        public ReadOnlyObservableCollection<TreeViewItem> Items { get; }

        public TreeView(string id) : base(id)
        {
            SelectedItems = new List<TreeViewItem>();
            _items = new ObservableCollection<TreeViewItem>();
            _items.CollectionChanged += Items_CollectionChanged;
            Items = new ReadOnlyObservableCollection<TreeViewItem>(_items);
        }

        public void Clear()
        {
            _items.Clear();
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
            if (list is null)
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
            if (list is null)
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

            if (item.HasValue() && item.ParentTree == this)
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

    [Obsolete]
    public abstract class TreeViewItem : BaseViewModel, ICloneable<TreeViewItem>
    {
        private bool _isExpanded;
        private bool _isSelected;
        private string _caption;
        private string _itemClass;
        private Glyph _glyph;
        private ContextMenu _contextMenu;

        public abstract bool CanExpand { get; }
        
        public virtual bool IsExpanded
        {
            [DebuggerStepThrough]
            get => _isExpanded;
            set
            {
                if (value)
                {     
                   Expand();
                }

                _isExpanded = value;
            }
        }
        
        public bool IsSelected
        {
            [DebuggerStepThrough]
            get => _isSelected;
            
            [DebuggerStepThrough]
            set => SetWithNotify(ref _isSelected, value);
        }
        
        /// <summary>
        /// Type of an item (e.g. 'drive', 'file' and 'directory' for file system objects)
        /// </summary>
        public string Caption
        {
            [DebuggerStepThrough]
            get => _caption;
            
            [DebuggerStepThrough]
            set => SetWithNotify(ref _caption, value, true);
        }
        
        public string ItemClass
        {
            [DebuggerStepThrough]
            get => _itemClass;
            
            [DebuggerStepThrough]
            set => SetWithNotify(ref _itemClass, value, true);
        }
        
        public Glyph ItemGlyph
        {
            [DebuggerStepThrough]
            get => _glyph;
            
            [DebuggerStepThrough]
            set => SetWithNotify(ref _glyph, value, true);
        }
        
        public TreeView ParentTree { get; set; }
        
        public TreeViewItem Parent { get; set; }
        
        public ContextMenu ContextMenu
        {
            [DebuggerStepThrough]
            get => _contextMenu;
            
            [DebuggerStepThrough]
            set => SetWithNotify(ref _contextMenu, value, true);
        }
        
        public ObservableCollection<TreeViewItem> Children { get; }

        public BaseCommand ExpandCommand { get; protected set; }

        public TreeViewItem()
        {
            Children = new ObservableCollection<TreeViewItem>();
        }

        [DebuggerStepThrough]
        public override string ToString() => $"tree-view-item :: {Caption} [{ItemClass}]";

        public TreeViewItem Clone()
        {
            var clone = CloneInternal();
            clone.Children.AddRange(Children.Select(item => item.Clone()));
            clone._caption = _caption;
            clone._itemClass = _itemClass;
            clone._glyph = _glyph;
            clone._contextMenu = _contextMenu;

            return clone;
        }
        
        [DebuggerStepThrough]
        public TreeViewItem Clone(string _) => Clone();

        protected virtual void OnChildrenClearing()
        {
            if (Children.Count != 0 && Children[0].HasValue())
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
                //if (Children.Count > 0 && Children[0].HasValue())
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