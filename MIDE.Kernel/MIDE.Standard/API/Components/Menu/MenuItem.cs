using System;
using System.Linq;
using MIDE.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace MIDE.API.Components
{
    public abstract class MenuItem : LayoutComponent
    {
        /// <summary>
        /// Minimum ordinal index for menu item, specify for the item that always should be the first one
        /// </summary>
        public const short MIN_ORDINAL = -99;
        /// <summary>
        /// Maximum ordinal index for menu item, specify for the item that always should be the last one
        /// </summary>
        public const short MAX_ORDINAL = 99;
        public const string DEFAULT_GROUP = "default";

        private List<MenuGroup> groups;

        public bool AllowIdDuplicates { get; }
        /// <summary>
        /// Ordinal index identifying item positioning in list
        /// </summary>
        public short OrdinalIndex { get; }
        public string Group { get; set; }
        public Menu ParentMenu { get; internal set; }
        public MenuItem ParentItem { get; internal set; }
        public ObservableCollection<MenuItem> Children { get; }

        public MenuItem this[string id] => Children.FirstOrDefault(i => i.Id == id);

        public MenuItem(string id, short ordinalIndex, bool allowDuplicates = false) : base(id)
        {
            Group = "default";
            AllowIdDuplicates = allowDuplicates;
            OrdinalIndex = ordinalIndex.Clamp(MIN_ORDINAL, MAX_ORDINAL);
            groups = new List<MenuGroup>();
            Children = new ObservableCollection<MenuItem>();
            Children.CollectionChanged += Children_CollectionChanged;

            groups.Add(new MenuGroup("default", -99, this));
        }
        
        /// <summary>
        /// Add the given item to child items. If the parent ID given, searches (recursively if the flag specified) an item with the specified ID
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <param name="parentId">The parent to add this item to</param>
        /// <param name="searchRecursively">Search parent recursively</param>
        public void Add(MenuItem item, string parentId, bool searchRecursively = false)
        {
            if (parentId == null)
            {
                Children.Insert(item, MIN_ORDINAL, MAX_ORDINAL);
                return;
            }

            var parent = Find(parentId, searchRecursively);
            if (parent == null)
                throw new ArgumentException($"The menu item with ID {parentId} not found");
            parent.Add(item, null);
        }
        public void AddGroup(MenuGroup group)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));
            if (group.Container != this)
                throw new ArgumentException("Cannot add menu group to {Id} because it is assigned to another container");
            if (groups.Contains(mg => mg.Id == group.Id))
                throw new ArgumentException($"Duplicate group ID on {Id} menu item");
            groups.Add(group);
            group.FirstItemIndex = 0;
            group.LastItemIndex = 0;
        }
        public void RemoveGroup(string id, bool removeItems = true)
        {
            int index = groups.FirstIndexWith(mg => mg.Id == id);
            if (index == -1)
                return;
            MenuGroup group = groups[index];
            //TODO: move items to another group
            if (removeItems)
            {
                //int len = group.ItemsCount;
                //for (int i = group.FirstItemIndex; i < len; i++)
                //{
                //    Children.RemoveAt(group.FirstItemIndex);
                //}
            }
            group.FirstItemIndex = 0;
            group.LastItemIndex = 0;
            groups.RemoveAt(index);
        }

        public MenuGroup GetMenuGroup(string id)
        {
            MenuGroup group = groups.FirstOrDefault(g => g.Id == id);
            return group;
        }
        public MenuItem Find(string id, bool searchRecursively = false)
        {
            foreach (var item in Children)
            {
                if (item.Id == id)
                    return item;
                if (searchRecursively)
                {
                    var inter = item.Find(id);
                    if (inter != null)
                        return inter;
                }
            }
            return null;
        }
        
        /// <summary>
        /// Searches for the last item in the given path and produces it's child items ordinal indexes
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public (string, short)[] GetItemsOrdinals() => Children.Select(mi => (mi.Id, mi.OrdinalIndex));

        /// <summary>
        /// Searches for the last item in the given path and produces out array of all child items ID
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string[] GetAllItemsIDs() => Children.Select(mi => mi.Id);

        protected override LayoutComponent CloneInternal(string id)
        {
            MenuItem clone = Create(id, OrdinalIndex, Group);
            clone.Group = Group;
            clone.groups.Clear();
            clone.groups.AddRange(groups);
            clone.Children.AddRange(Children.Select(item => item.Clone() as MenuItem));
            return clone;
        }
        protected abstract MenuItem Create(string id, short ordinalIndex, string group);

        #region Collection Changes
        private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    OnElementsAdd(e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    OnElementsRemove(e.OldItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    OnElementsReplace(e.NewItems, e.OldItems);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    OnElementsRemove(e.OldItems);
                    break;
            }
        }

        private void OnElementsAdd(IList items)
        {
            foreach (var item in items)
            {
                var menuItem = item as MenuItem;
                OnElementAdd(menuItem);
            }
        }
        private void OnElementsRemove(IList items)
        {
            foreach (var item in items)
            {
                var menuItem = item as MenuItem;
                OnElementRemove(menuItem);
            }
        }
        private void OnElementsReplace(IList newItems, IList oldItems)
        {
            foreach (var item in oldItems)
            {
                var menuItem = item as MenuItem;
                OnElementRemove(menuItem);
            }
            foreach (var item in newItems)
            {
                var menuItem = item as MenuItem;
                OnElementAdd(menuItem);
            }
        }
        private void OnCollectionReset(IList oldItems)
        {
            foreach (var item in oldItems)
            {
                var menuItem = item as MenuItem;
                OnElementRemove(menuItem);
            }
        }

        private void OnElementAdd(MenuItem menuItem)
        {
            if (Children.Count(i => i.Id == menuItem.Id) > 1)
                throw new InvalidOperationException("Collection can not contain duplicate entries");
            menuItem.Parent = this;
            menuItem.ParentMenu = ParentMenu;
            menuItem.ParentItem = null;
        }
        private void OnElementRemove(MenuItem menuItem)
        {
            menuItem.Parent = null;
            menuItem.ParentMenu = null;
            menuItem.ParentItem = null;
        } 
        #endregion
    }
}