using System;
using System.Linq;
using System.Collections;
using MIDE.Standard.Helpers;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace MIDE.Standard.API.Components
{
    public class MenuGroup : MenuItem
    {
        public bool AllowIdDuplicates { get; }
        public ObservableCollection<MenuItem> Children { get; }

        public override MenuItem this[string id] => Children.FirstOrDefault(i => i.Id == id);

        public MenuGroup(string id, bool allowDuplicates = false) : base(id)
        {
            AllowIdDuplicates = allowDuplicates;
            Children = new ObservableCollection<MenuItem>();
            Children.CollectionChanged += Children_CollectionChanged;
        }

        public override void Add(MenuItem item, string parentId)
        {
            if (parentId == null)
            {
                Children.Add(item);
                return;
            }

            var parent = Find(parentId);
            if (parent == null)
                throw new ArgumentException($"The menu item with ID {parentId} not found");
            parent.Add(item, null);
        }
        public override void AddAfter(MenuItem item, string id)
        {
            var prevIndex = Children.IndexOf(i => i.Id == id);
            if (prevIndex == -1)
                throw new ArgumentException($"Menu item with ID {id} not found");
            if (prevIndex == Children.Count - 1)
            {
                Children.Add(item);
                return;
            }
            Children.Insert(prevIndex, item);
        }
        public override void AddBefore(MenuItem item, string id)
        {
            var prevIndex = Children.IndexOf(i => i.Id == id);
            if (prevIndex == -1)
                throw new ArgumentException($"Menu item with ID {id} not found");
            Children.Insert(prevIndex, item);
        }

        public override MenuItem Find(string id)
        {
            foreach (var item in Children)
            {
                if (item.Id == id)
                    return item;
                var inter = item.Find(id);
                if (inter != null)
                    return inter;
            }
            return null;
        }

        private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    OnElementsAdd(e.NewItems);
                    break;
            }
        }
        private void OnElementsAdd(IList items)
        {
            if (AllowIdDuplicates)
                return;
            foreach (var item in items)
            {
                var menuItem = item as MenuItem;
                if (Children.Count(i => i.Id == menuItem.Id) > 1)
                    throw new InvalidOperationException("Collection can not contain duplicate entries");
                menuItem.Parent = this;
                menuItem.ParentItem = this;
                menuItem.ParentMenu = ParentMenu;
            }
        }
    }
}