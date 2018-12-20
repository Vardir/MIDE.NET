using System;
using System.Linq;
using System.Collections;
using MIDE.Standard.Helpers;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace MIDE.Standard.API.Components
{
    public class Menu : LayoutContainer, IMenuConstructionContext
    {
        public const string PATH_PATTERN = "^(" + ID_PATTERN_CLEAN + ")(?:/(" + ID_PATTERN_CLEAN + "))*$";

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
            Items.Add(menuItem);
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
        public void AddItem(MenuItem item) => Items.Add(item);
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
                AddItem(item);
                return;
            }
            if (!Regex.IsMatch(path, PATH_PATTERN))
                throw new FormatException("Path has invalid format");
            var (rootId, tail) = path.ExtractUntil(0, '/');
            var root = Items.FirstOrDefault(i => i.Id == rootId);
            if (root == null)
            {
                root = new MenuButton(rootId, false);
                AddItem(root);
            }
            string segment = tail;
            while (!string.IsNullOrEmpty(segment))
            {
                var (elementId, tail2) = segment.ExtractUntil(0, '/');
                var element = root[elementId];
                if (element == null)
                {
                    element = new MenuButton(elementId, false);
                    root.Add(element, null);
                }
                root = element;
                segment = tail2;
            }
            root.Add(item, null);
        }
        /// <summary>
        /// Adds the given menu item in the list after the element with specified ID
        /// </summary>
        /// <param name="item"></param>
        /// <param name="id"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void AddAfter(MenuItem item, string id)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var prevIndex = Items.IndexOf(i => i.Id == id);
            if (prevIndex == -1)
                throw new ArgumentException($"Menu item with ID {id} not found");
            prevIndex++;            
            if (prevIndex == Items.Count)
            {
                Items.Add(item);
                return;
            }
            Items.Insert(prevIndex, item);
        }
        /// <summary>
        /// Adds the given menu item in the list before the element with specified ID
        /// </summary>
        /// <param name="item"></param>
        /// <param name="id"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void AddBefore(MenuItem item, string id)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var prevIndex = Items.IndexOf(i => i.Id == id);
            if (prevIndex == -1)
                throw new ArgumentException($"Menu item with ID {id} not found");
            Items.Insert(prevIndex, item);
        }
        /// <summary>
        /// Adds the given menu item into the list of the specified parent after the element with specified ID
        /// </summary>
        /// <param name="item"></param>
        /// <param name="id"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void AddAfter(MenuItem item, string id, string parentId)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var parent = Find(parentId) as MenuItem;
            if (parent == null)
                throw new ArgumentException($"Menu item with ID {parentId} not found");
            parent.AddAfter(item, id);
        }
        /// <summary>
        /// Adds the given menu item into the list of the specified parent before the element with specified ID
        /// </summary>
        /// <param name="item"></param>
        /// <param name="id"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void AddBefore(MenuItem item, string id, string parentId)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var parent = Find(parentId) as MenuItem;
            if (parent == null)
                throw new ArgumentException($"Menu item with ID {parentId} not found");
            parent.AddBefore(item, id);
        }

        public override bool Contains(string id) => Items.FirstOrDefault(i => i.Id == id) != null;
        /// <summary>
        /// Searches recursively for the menu item with specified ID. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
            foreach (var item in items)
            {
                var menuItem = item as MenuItem;
                if (Items.Count(i => i.Id == menuItem.Id) > 1)
                    throw new InvalidOperationException("Collection can not contain duplicate entries");
                menuItem.Parent = this;
                menuItem.ParentMenu = this;
                menuItem.ParentItem = null;
            }
        }
    }
}