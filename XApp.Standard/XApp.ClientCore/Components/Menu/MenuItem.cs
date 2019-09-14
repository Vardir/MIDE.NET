using System;
using System.Linq;
using System.Collections.Generic;

using XApp.API;
using XApp.Helpers;

namespace XApp.Components
{
    public abstract class MenuItem : LayoutComponent, IOrderable
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

        private string caption;
        private List<MenuGroup> menuGroups;

        public int ChildCount { get; private set; }

        /// <summary>
        /// Ordinal index that corresponds to ordering position of item in collection
        /// </summary>
        public short OrdinalIndex { get; }
        public string Caption
        {
            get => caption;
            set => SetLocalizedAndNotify(value, ref caption);
        }

        /// <summary>
        /// A menu group ID this item assigned to
        /// </summary>
        public string Group { get; internal set; }

        /// <summary>
        /// A menu this item belongs to
        /// </summary>
        public Menu ParentMenu { get; internal set; }

        /// <summary>
        /// Parent item that contains this item
        /// </summary>
        public MenuItem ParentItem { get; internal set; }

        /// <summary>
        /// An enumeration of all child items (null value represents a separator between two menu groups)
        /// </summary>
        public IEnumerable<MenuItem> Children
        {
            get
            {
                for (int i = 0; i < menuGroups.Count; i++)
                {
                    foreach (var item in menuGroups[i].Items)
                    {
                        yield return item;
                    }

                    if (menuGroups[i].Items.Count > 0 && i < menuGroups.Count - 1 
                                                      && menuGroups[i + 1].Items.Count > 0)
                    {
                        yield return new MenuSplitter("splitter", 0);
                    }
                }
            }
        }

        public MenuItem this[string id]
        {
            get
            {
                for (int i = 0; i < menuGroups.Count; i++)
                {
                    foreach (var item in menuGroups[i].Items)
                    {
                        if (item.Id == id)
                            return item;
                    }
                }

                return null;
            }
        }

        public MenuItem(string id, short ordinalIndex) : base(id)
        {
            Caption = $"({id})";
            OrdinalIndex = ordinalIndex.Clamp(MIN_ORDINAL, MAX_ORDINAL);
            menuGroups = new List<MenuGroup>(1)
            {
                new MenuGroup(DEFAULT_GROUP, -99)
            };
        }
        public MenuItem(string id, string group, short ordinalIndex) : this(id, ordinalIndex)
        {
            Group = group;
        }

        /// <summary>
        /// Makes copy of items from source and adds them to destination
        /// </summary>
        /// <param name="fdscheme"></param>
        public void MergeWith(MenuItem source)
        {
            for (int i = 0; i < source.menuGroups.Count; i++)
            {
                var group = source.menuGroups[i];
                var innerGroup = menuGroups.Find(mg => mg.Id == group.Id);

                if (innerGroup == null)
                    AddGroup(group.Id, group.OrdinalIndex);

                foreach (var item in group.Items)
                {
                    if (innerGroup == null || !innerGroup.Items.Contains(mi => mi.Id == item.Id))
                        Add(item.Clone() as MenuItem, null);
                }
            }
        }

        /// <summary>
        /// Add the given item to child items. If the parent ID given, firstly searches for group with that ID. If group not found than searches (recursively if the flag specified) an item with the specified ID
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <param name="parentId">The parent to add this item to</param>
        /// <param name="searchRecursively">Search parent recursively</param>
        public void Add(MenuItem item, string parentId, bool searchRecursively = false)
        {
            if (item.Group == null)
                item.Group = DEFAULT_GROUP;

            MenuGroup group;
            if (parentId == null)
            {
                group = menuGroups.Find(g => g.Id == item.Group);
                if (group == null)
                {
                    item.Group = DEFAULT_GROUP;
                    group = menuGroups[0];
                }

                group.Items.Insert(item);
                ChildCount++;

                return;
            }

            group = menuGroups.Find(mg => mg.Id == parentId);
            if (group.HasValue())
            {
                item.Group = parentId;
                group.Items.Insert(item);
                ChildCount++;

                return;
            }

            var parent = Find(parentId, searchRecursively);
            if (parent == null)
                throw new ArgumentException($"The menu item with ID {parentId} not found");

            parent.Add(item, null);
        }

        /// <summary>
        /// Creates a menu group
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ordinalIndex"></param>
        public void AddGroup(string id, short ordinalIndex)
        {
            var group = new MenuGroup(id, ordinalIndex);
            menuGroups.Insert(group);
        }
        public void RemoveItem(string id)
        {
            var item = this[id];
            if (item.HasValue())
            {
                var group = menuGroups.Find(g => g.Id == item.Group);
                if (group == null)
                    throw new InvalidOperationException("An item's group ID was changed");

                ChildCount--;
                group.Items.Remove(item);
            }
        }
        /// <summary>
        /// Removes an existing menu group. If <paramref name="removeItems"/> parameter is false, moves all group's items to default group
        /// <para>If the given ID is <see cref="DEFAULT_GROUP"/>nothing changes</para>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="removeItems"></param>
        public void RemoveGroup(string id, bool removeItems = true)
        {
            if (id == DEFAULT_GROUP)
                return;

            int index = menuGroups.FirstIndexWith(mg => mg.Id == id);
            if (index == -1)
                return;

            var group = menuGroups[index];
            if (removeItems)
            {
                ChildCount -= group.Items.Count;
            }
            else
            {
                foreach (var item in group.Items)
                {
                    item.Group = DEFAULT_GROUP;
                    menuGroups[0].Items.Insert(item);
                }
            }

            menuGroups.RemoveAt(index);
        }

        public bool ContainsGroup(string id) => menuGroups.Contains(mg => mg.Id == id);
        public MenuItem Find(string id, bool searchRecursively = false)
        {
            foreach (var item in Children)
            {
                if (item.Id == id)
                    return item;

                if (searchRecursively)
                {
                    var inter = item.Find(id);
                    if (inter.HasValue())
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
        public (string, short)[] GetItemsOrdinals()
        {
            var array = new (string, short)[menuGroups.Sum(g => g.Items.Count)];
            for (int i = 0, k = 0; i < menuGroups.Count; i++)
            {
                var items = menuGroups[i].Items;
                for (int j = 0; j < items.Count; j++, k++)
                {
                    array[k] = (items[j].Id, items[j].OrdinalIndex);
                }
            }

            return array;
        }

        /// <summary>
        /// Searches for the last item in the given path and produces out array of all child items ID
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string[] GetAllItemsIDs()
        {
            string[] array = new string[menuGroups.Sum(g => g.Items.Count)];
            for (int i = 0, k = 0; i < menuGroups.Count; i++)
            {
                var items = menuGroups[i].Items;
                for (int j = 0; j < items.Count; j++, k++)
                {
                    array[k] = items[j].Id;
                }
            }

            return array;
        }

        protected override LayoutComponent CloneInternal(string id)
        {
            var clone = Create(id, OrdinalIndex, Group);
            clone.Group = Group;
            clone.menuGroups.Clear();
            clone.menuGroups.AddRange(menuGroups.Select(g => g.Clone()));

            return clone;
        }
        protected abstract MenuItem Create(string id, short ordinalIndex, string group);

        protected class MenuGroup : ApplicationComponent, ICloneable<MenuGroup>, IOrderable
        {
            public const short MIN_ORDINAL = -8;
            public const short MAX_ORDINAL = 8;

            public short OrdinalIndex { get; }
            public List<MenuItem> Items { get; }

            public MenuGroup(string id, short ordinalIndex) : base(id)
            {
                OrdinalIndex = ordinalIndex.Clamp(MIN_ORDINAL, MAX_ORDINAL);
                Items = new List<MenuItem>();
            }

            public MenuGroup Clone()
            {
                var clone = new MenuGroup(Id, OrdinalIndex);
                clone.Items.AddRange(Items);
                return clone;
            }
            public MenuGroup Clone(string id)
            {
                var clone = new MenuGroup(id, OrdinalIndex);
                clone.Items.AddRange(Items);
                return clone;
            }
        }
    }
}