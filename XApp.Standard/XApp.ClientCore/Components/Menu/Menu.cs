using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

using Vardirsoft.Shared.Helpers;
using Vardirsoft.XApp.Helpers;

namespace Vardirsoft.XApp.Components
{
    public class Menu : ApplicationComponent, IMenuConstructionContext
    {
        private readonly List<MenuItem> _items;

        public const string PATH_PATTERN = "^(" + ID_PATTERN_INL + ")(?:/(" + ID_PATTERN_INL + "))*$";

        public int ItemsCount { [DebuggerStepThrough] get => _items.Count; }
        
        public IEnumerable<MenuItem> Items { [DebuggerStepThrough] get => _items; }

        public MenuItem this[string id] { [DebuggerStepThrough] get => _items.FirstOrDefault(i => i.Id == id); }

        public Menu(string id) : base(id)
        {
            _items = new List<MenuItem>();
        }

        /// <summary>
        /// Makes copy of items from source and adds them to destination
        /// </summary>
        /// <param name="source"></param>
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
            // if (IsSealed)
            //     throw new InvalidOperationException("Attempt to add item to sealed collection");

            _items.Insert(item);
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
            // if (IsSealed)
            //     throw new InvalidOperationException("Attempt to add item to sealed collection");

            Guard.EnsureNonEmpty(path, typeof(ArgumentException), "Path can not be empty");

            if (path == "/")
            {
                _items.Insert(item);
            }
            else
            {
                Guard.Ensure(Regex.IsMatch(path, PATH_PATTERN), typeof(FormatException), "Invalid path format");

                var (rootId, tail) = path.ExtractUntil(0, '/');
                var root = this[rootId];
                if (root is null)
                {
                    root = new MenuButton(rootId, 0);
                    _items.Insert(item);
                }

                var last = GetItem(root, tail, true);
                last.Add(item, null);
            }
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
            Guard.EnsureNonEmpty(path, typeof(ArgumentException), "The path can not be empty");

            if (path == "/")
                return true;

            Guard.Ensure(Regex.IsMatch(path, PATH_PATTERN), typeof(FormatException), "Invalid path format");

            var (rootId, tail) = path.ExtractUntil(0, '/');
            var root = this[rootId];

            if (root is null)
                return false;

            return GetItem(root, tail, false).HasValue();
        }
        
        public bool Contains(string id) => _items.FirstOrDefault(i => i.Id == id).HasValue();

        /// <summary>
        /// Searches recursively for the menu item with specified ID. Returns null if nothing found
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FormatException"></exception>
        public ApplicationComponent Find(string id)
        {
            foreach (var item in _items)
            {
                if (item.Id == id)
                    return item;

                var inter = item.Find(id);
                if (inter.HasValue())
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
            Guard.EnsureNonEmpty(path, typeof(ArgumentException), "Path can not be empty");

            if (path == "/")
                return _items.Select(mi => mi.Id);

            Guard.Ensure(Regex.IsMatch(path, PATH_PATTERN), typeof(FormatException), "Invalid path format");

            var (rootId, tail) = path.ExtractUntil(0, '/');
            var root = this[rootId];
            
            if (root is null)
                return null;

            var item = GetItem(root, tail, false);

            return item?.GetAllItemsIDs();
        }

        /// <summary>
        /// Searches for the last item in the given path and produces it's child items ordinal indexes. Returns null if nothing found
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FormatException"></exception>
        public (string id, int ordinalIndex)[] GetItemsOrdinals(string path)
        {
            Guard.EnsureNonEmpty(path, typeof(ArgumentException), "Path can not be empty");

            if (path == "/")
                return _items.Select(mi => (mi.Id, mi.OrdinalIndex));

            Guard.Ensure(Regex.IsMatch(path, PATH_PATTERN), typeof(FormatException), "Invalid path format");

            var (rootId, tail) = path.ExtractUntil(0, '/');
            var root = this[rootId];
            
            if (root is null)
                return null;

            var segment = tail;
            while (segment.HasValue())
            {
                var (elementId, tail2) = segment.ExtractUntil(0, '/');
                var element = root[elementId];
                
                if (element is null)
                    return null;

                root = element;
                segment = tail2;
            }

            return root.GetItemsOrdinals();
        }

        protected override ApplicationComponent CloneInternal(string id)
        {
            var clone = new Menu(id);
            clone._items.AddRange(_items.Select(item => item.Clone() as MenuItem));

            return clone;
        }
        
        private MenuItem GetItem(MenuItem root, string path, bool createIfNotExist)
        {
            if (string.IsNullOrEmpty(path))
                return root;

            var (segment, tail) = path.ExtractUntil(0, '/');
            if (root.ContainsGroup(segment))
                return GetItem(root, tail, createIfNotExist);

            var item = root.Find(segment);
            if (item is null && !createIfNotExist)
                return null;

            if (item is null)
            {
                item = new MenuButton(segment, 0);
                root.Add(item, null);
            }

            if (tail is null)
                return item;

            return GetItem(item, tail, createIfNotExist);
        }
    }
}