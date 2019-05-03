using System;
using System.Linq;
using MIDE.Helpers;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MIDE.API.Components
{
    public class Menu : LayoutContainer, IMenuConstructionContext
    {
        private List<MenuItem> items;

        public const string PATH_PATTERN = "^(" + ID_PATTERN_INL + ")(?:/(" + ID_PATTERN_INL + "))*$";

        public IEnumerable<MenuItem> Items => items;

        public MenuItem this[string id] => items.FirstOrDefault(i => i.Id == id);

        public Menu(string id) : base(id)
        {
            items = new List<MenuItem>();
        }

        /// <summary>
        /// Makes copy of items from source and adds them to destination
        /// </summary>
        /// <param name="fdscheme"></param>
        public void MergeWith(Menu source)
        {
            foreach (var item in source.Items)
            {
                var copy = item.Clone() as MenuItem;
                AddItem(copy);
            }
        }
        public void AddItem(MenuItem item)
        {
            if (IsSealed)
                throw new InvalidOperationException("Attempt to add item to sealed collection");
            items.Insert(item);
        }
        /// <summary>
        /// Puts the specified element to the last one element in path. Each non-existing element in path will be created.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="item"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FormatException"></exception>
        public void AddItem(string path, MenuItem item)
        {
            if (IsSealed)
                throw new InvalidOperationException("Attempt to add item to sealed collection");
            if (string.IsNullOrWhiteSpace(path) || path.Length == 0)
                throw new ArgumentException("The path can not be empty");
            if (path == "/")
            {
                items.Insert(item);
                return;
            }
            if (!Regex.IsMatch(path, PATH_PATTERN))
                throw new FormatException("Path has invalid format");

            var (rootId, tail) = path.ExtractUntil(0, '/');
            var root = this[rootId];
            if (root == null)
            {
                root = new MenuButton(rootId, 0);
                items.Insert(item);
            }
            string segment = tail;
            while (!string.IsNullOrEmpty(segment))
            {
                var (elementId, tail2) = segment.ExtractUntil(0, '/');
                var element = root[elementId];
                if (element == null)
                {
                    element = new MenuButton(elementId, 0);
                    root.Add(element, null);
                }
                root = element;
                segment = tail2;
            }
            root.Add(item, null);
        }

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
        public override bool Contains(string id) => items.FirstOrDefault(i => i.Id == id) != null;
        /// <summary>
        /// Searches recursively for the menu item with specified ID. Returns null if nothing found
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FormatException"></exception>
        public override LayoutComponent Find(string id)
        {
            foreach (var item in items)
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
                return items.Select(mi => mi.Id);

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
        /// <summary>
        /// Searches for the last item in the given path and produces it's child items ordinal indexes. Returns null if nothing found
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FormatException"></exception>
        public (string id, short ordinalIndex)[] GetItemsOrdinals(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || path.Length == 0)
                throw new ArgumentException("The path can not be empty");
            if (path == "/")
                return items.Select(mi => (mi.Id, mi.OrdinalIndex));

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

        protected override void AddChild_Impl(LayoutComponent component)
        {
            if (!(component is MenuItem item))
                throw new ArgumentException($"Expected [MenuItem] but got [{component.GetType()}]");
            items.Insert(item);
        }
        protected override void RemoveChild_Impl(string id)
        {
            int index = items.FirstIndexWith(i => i.Id == id);
            if (index == -1)
                return;
            items.RemoveAt(index);
        }
        protected override void RemoveChild_Impl(LayoutComponent component)
        {
            int index = items.FirstIndexWith(i => i == component);
            if (index == -1)
                return;
            items.RemoveAt(index);
        }

        protected override LayoutComponent CloneInternal(string id)
        {
            Menu clone = new Menu(id);
            clone.items.AddRange(items.Select(item => item.Clone() as MenuItem));
            return clone;
        }
    }
}