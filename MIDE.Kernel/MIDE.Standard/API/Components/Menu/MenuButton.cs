using MIDE.Helpers;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace MIDE.API.Components
{
    public class MenuButton : MenuItem
    {
        public bool AllowIdDuplicates { get; }
        public ObservableCollection<MenuItem> Children { get; }

        public override MenuItem this[string id] => Children.FirstOrDefault(i => i.Id == id);

        public MenuButton(string id, short ordinalIndex, bool allowDuplicates = false) : base(id, ordinalIndex)
        {
            AllowIdDuplicates = allowDuplicates;
            Children = new ObservableCollection<MenuItem>();
            Children.CollectionChanged += Children_CollectionChanged;
        }

        public override void Press(object parameter)
        {
            if (Children.Count > 0)
                return;
            if (PressCommand.CanExecute(parameter))
                PressCommand.Execute(parameter);
        }
        public override void Add(MenuItem item, string parentId, bool searchRecursively = false)
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

        public override MenuItem Find(string id, bool searchRecursively = false)
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
        public override (string, short)[] GetItemsOrdinals() => Children.Select(mi => (mi.Id, mi.OrdinalIndex));
        public override string[] GetAllItemsIDs() => Children.Select(mi => mi.Id);

        protected override Button Create(string id)
        {
            MenuButton clone = new MenuButton(id, OrdinalIndex, AllowIdDuplicates);
            clone.Children.AddRange(Children.Select(item => item.Clone() as MenuItem));
            return clone;
        }
        protected override MenuItem Create(string id, short ordinalIndex)
        {
            MenuButton clone = new MenuButton(id, ordinalIndex, AllowIdDuplicates);
            clone.Children.AddRange(Children.Select(item => item.Clone() as MenuItem));
            return clone;
        }

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
    }
}