using System;
using System.Linq;
using System.Collections;
using MIDE.Standard.Helpers;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace MIDE.Standard.API.Components
{
    public class Menu : LayoutContainer
    {
        public ObservableCollection<MenuItem> MenuItems { get; }

        public const string PATH_PATTERN = "^(" + ID_PATTERN_CLEAN + ")(?:/(" + ID_PATTERN_CLEAN + "))*$";

        public Menu(string id) : base(id)
        {
            MenuItems = new ObservableCollection<MenuItem>();
            MenuItems.CollectionChanged += MenuItems_CollectionChanged;
        }
        
        public override void RemoveChild(string id)
        {
            var item = MenuItems.FirstOrDefault(i => i.Id == id);
            MenuItems.Remove(item);
        }
        public override void AddChild(LayoutComponent component)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));
            if (!(component is MenuItem menuItem))
                throw new ArgumentException($"Menu can not contain elements of type {component.GetType()}");
            MenuItems.Add(menuItem);
        }
        public override void RemoveChild(LayoutComponent component)
        {
            if (!(component is MenuItem menuItem))
                throw new ArgumentException($"Menu does not contain elements of type {component.GetType()}");
            MenuItems.Remove(menuItem);
        }
        public void AddChild(MenuItem item) => MenuItems.Add(item);
        public void AddChild(string path, MenuItem item)
        {
            if (string.IsNullOrWhiteSpace(path) || path.Length == 0)
                throw new ArgumentException("The path can not be empty");
            if (path == "/")
            {
                AddChild(item);
                return;
            }
            if (!Regex.IsMatch(path, PATH_PATTERN))
                throw new FormatException("Path has invalid format");
            var (rootId, tail) = path.ExtractUntil(0, '/');
            var root = MenuItems.FirstOrDefault(i => i.Id == rootId);
            if (root == null)
            {
                root = new MenuButton(rootId, false);
                AddChild(root);
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
        public void AddAfter(MenuItem item, string id)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var prevIndex = MenuItems.IndexOf(i => i.Id == id);
            if (prevIndex == -1)
                throw new ArgumentException($"Menu item with ID {id} not found");
            prevIndex++;            
            if (prevIndex == MenuItems.Count)
            {
                MenuItems.Add(item);
                return;
            }
            MenuItems.Insert(prevIndex, item);
        }
        public void AddBefore(MenuItem item, string id)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var prevIndex = MenuItems.IndexOf(i => i.Id == id);
            if (prevIndex == -1)
                throw new ArgumentException($"Menu item with ID {id} not found");
            MenuItems.Insert(prevIndex, item);
        }
        public void AddAfter(MenuItem item, string id, string parentId)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var parent = Find(parentId) as MenuItem;
            if (parent == null)
                throw new ArgumentException($"Menu item with ID {parentId} not found");
            parent.AddAfter(item, id);
        }
        public void AddBefore(MenuItem item, string id, string parentId)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var parent = Find(parentId) as MenuItem;
            if (parent == null)
                throw new ArgumentException($"Menu item with ID {parentId} not found");
            parent.AddBefore(item, id);
        }

        public override bool Contains(string id) => MenuItems.FirstOrDefault(i => i.Id == id) != null;
        public override LayoutComponent Find(string id)
        {
            foreach (var item in MenuItems)
            {
                if (item.Id == id)
                    return item;
                var inter = item.Find(id);
                if (inter != null)
                    return inter;
            }
            return null;
        }

        private void MenuItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
                if (MenuItems.Count(i => i.Id == menuItem.Id) > 1)
                    throw new InvalidOperationException("Collection can not contain duplicate entries");
                menuItem.Parent = this;
                menuItem.ParentMenu = this;
                menuItem.ParentItem = null;
            }
        }
    }
}