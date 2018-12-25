using System;
using System.Linq;
using System.Collections;
using MIDE.Standard.Helpers;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace MIDE.Standard.API.Components
{
    /// <summary>
    /// The main application menu
    /// </summary>
    public class Menu : LayoutContainer, IMenuConstructionContext
    {
        /// <summary>
        /// The RegEx pattern used to match string to path pattern that can contain single item or sequential items representing path
        /// </summary>
        public const string PATH_PATTERN = "^(" + ID_PATTERN_INL + ")(?:/(" + ID_PATTERN_INL + "))*$";

        public ObservableCollection<MenuItem> Items { get; }

        public MenuItem this[string id] => Items.FirstOrDefault(i => i.Id == id);

        public Menu(string id) : base(id)
        {
            Items = new ObservableCollection<MenuItem>();
            Items.CollectionChanged += Items_CollectionChanged;
        }
        
        public override void RemoveChild(string id)
        {
            var item = Items.FirstOrDefault(i => i.Id == id);
            Items.Remove(item);
        }
        /// <summary>
        /// Adds the specified element into the list of menu items. If the component is not subtype of MenuItem exception thrown
        /// </summary>
        /// <param name="component"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public override void AddChild(LayoutComponent component)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));
            if (!(component is MenuItem menuItem))
                throw new ArgumentException($"Menu can not contain elements of type {component.GetType()}");

            Items.Insert(menuItem, MenuItem.MIN_ORDINAL, MenuItem.MAX_ORDINAL);
        }
        /// <summary>
        /// Removes the component from the list of menu items.
        /// </summary>
        /// <param name="component"></param>
        /// <exception cref="ArgumentException"></exception>
        public override void RemoveChild(LayoutComponent component)
        {
            if (!(component is MenuItem menuItem))
                throw new ArgumentException($"Menu does not contain elements of type {component.GetType()}");
            Items.Remove(menuItem);
        }
        public void AddItem(MenuItem item) => Items.Insert(item, MenuItem.MIN_ORDINAL, MenuItem.MAX_ORDINAL);
        /// <summary>
        /// Puts the specified element to the last one element in path. Each non-existing element in path will be created.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="item"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FormatException"></exception>
        public void AddItem(string path, MenuItem item)
        {
            if (string.IsNullOrWhiteSpace(path) || path.Length == 0)
                throw new ArgumentException("The path can not be empty");
            if (path == "/")
            {
                Items.Insert(item, MenuItem.MIN_ORDINAL, MenuItem.MAX_ORDINAL);
                return;
            }
            if (!Regex.IsMatch(path, PATH_PATTERN))
                throw new FormatException("Path has invalid format");

            var (rootId, tail) = path.ExtractUntil(0, '/');
            var root = this[rootId];
            if (root == null)
            {
                root = new MenuButton(rootId, 0, false);
                Items.Insert(item, MenuItem.MIN_ORDINAL, MenuItem.MAX_ORDINAL);
            }
            string segment = tail;
            while (!string.IsNullOrEmpty(segment))
            {
                var (elementId, tail2) = segment.ExtractUntil(0, '/');
                var element = root[elementId];
                if (element == null)
                {
                    element = new MenuButton(elementId, 0, false);
                    root.Add(element, null);
                }
                root = element;
                segment = tail2;
            }
            root.Add(item, null);
        }
       
        public override bool Contains(string id) => Items.FirstOrDefault(i => i.Id == id) != null;
        /// <summary>
        /// Verifies if the given path is exist in the menu and it's sub-items
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FormatException"></exception>
        public bool Exists(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || path.Length == 0)
                throw new ArgumentException("The path can not be empty");
            if (path == "/")
                return true;
            if (!Regex.IsMatch(path, PATH_PATTERN))
                throw new FormatException("Path has invalid format");

            var (rootId, tail) = path.ExtractUntil(0, '/');
            var root = this[rootId];
            if (root == null)
                return false;
            string segment = tail;
            while (!string.IsNullOrEmpty(segment))
            {
                var (elementId, tail2) = segment.ExtractUntil(0, '/');
                var element = root[elementId];
                if (element == null)
                    return false;
                root = element;
                segment = tail2;
            }
            return true;
        }
        /// <summary>
        /// Searches recursively for the menu item with specified ID. Returns null if nothing found
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FormatException"></exception>
        public override LayoutComponent Find(string id)
        {
            foreach (var item in Items)
            {
                if (item.Id == id)
                    return item;
                var inter = item.Find(id);
                if (inter != null)
                    return inter;
            }
            return null;
        }
        /// <summary>
        /// Searches for the last item in the given path and produces it's child items ordinal indexes. Returns null if nothing found
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FormatException"></exception>
        public (string, short)[] GetItemsOrdinals(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || path.Length == 0)
                throw new ArgumentException("The path can not be empty");
            if (path == "/")
                return Items.Select(mi => (mi.Id, mi.OrdinalIndex));

            if (!Regex.IsMatch(path, PATH_PATTERN))
                throw new FormatException("Path has invalid format");

            var (rootId, tail) = path.ExtractUntil(0, '/');
            var root = this[rootId];
            if (root == null)
                return null;
            string segment = tail;
            while (!string.IsNullOrEmpty(segment))
            {
                var (elementId, tail2) = segment.ExtractUntil(0, '/');
                var element = root[elementId];
                if (element == null)
                    return null;
                root = element;
                segment = tail2;
            }
            return root.GetItemsOrdinals();
        }
        /// <summary>
        /// Searches for the last item in the given path and produces out array of all child items ID. Returns null if nothing found
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FormatException"></exception>
        public string[] GetAllPaths(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || path.Length == 0)
                throw new ArgumentException("The path can not be empty");
            if (path == "/")
                return Items.Select(mi => mi.Id);

            if (!Regex.IsMatch(path, PATH_PATTERN))
                throw new FormatException("Path has invalid format");

            var (rootId, tail) = path.ExtractUntil(0, '/');
            var root = this[rootId];
            if (root == null)
                return null;
            string segment = tail;
            while (!string.IsNullOrEmpty(segment))
            {
                var (elementId, tail2) = segment.ExtractUntil(0, '/');
                var element = root[elementId];
                if (element == null)
                    return null;
                root = element;
                segment = tail2;
            }
            return root.GetAllItemsIDs();
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
            if (Items.Count(i => i.Id == menuItem.Id) > 1)
                throw new InvalidOperationException("Collection can not contain duplicate entries");
            menuItem.Parent = this;
            menuItem.ParentMenu = this;
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