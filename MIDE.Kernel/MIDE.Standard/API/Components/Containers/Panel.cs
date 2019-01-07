using System;
using System.Linq;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace MIDE.API.Components
{
    public class Panel : LayoutContainer
    {
        public ObservableCollection<LayoutComponent> Children { get; }

        public LayoutComponent this[string id] => Children.FirstOrDefault(c => c.Id == id);

        public Panel(string id) : base(id)
        {
            Children = new ObservableCollection<LayoutComponent>();
            Children.CollectionChanged += Items_CollectionChanged;
        }

        /// <summary>
        /// Adds the specified component to the collection
        /// </summary>
        /// <param name="component"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public override void AddChild(LayoutComponent component)
        {
            if (component == null)
                throw new ArgumentNullException("The argument should not be null");
            if (Children.FirstOrDefault(c => c.Id == component.Id) != null)
                throw new ArgumentException("The component with the same ID is already in the collection");
            Children.Add(component);
        }

        /// <summary>
        /// Searches child with the specified ID and removes it from the collection
        /// </summary>
        /// <param name="id"></param>
        public override void RemoveChild(string id)
        {
            var child = Children.FirstOrDefault(c => c.Id == id);
            if (child != null)
                RemoveChild(child);
        }

        /// <summary>
        /// Removes the specified component from the collection
        /// </summary>
        /// <param name="component"></param>
        public override void RemoveChild(LayoutComponent component)
        {
            Children.Remove(component);
            component.Parent = null;
        }

        /// <summary>
        /// Checks whether the item with the specified ID is in collection
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool Contains(string id) => Children.FirstOrDefault(c => c.Id == id) != null;

        /// <summary>
        /// Searches child with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override LayoutComponent Find(string id) => Children.FirstOrDefault(c => c.Id == id);

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
                var component = item as LayoutComponent;
                OnElementAdd(component);
            }
        }
        private void OnElementsRemove(IList items)
        {
            foreach (var item in items)
            {
                var component = item as LayoutComponent;
                OnElementRemove(component);
            }
        }
        private void OnElementsReplace(IList newItems, IList oldItems)
        {
            foreach (var item in oldItems)
            {
                var component = item as LayoutComponent;
                OnElementRemove(component);
            }
            foreach (var item in newItems)
            {
                var component = item as LayoutComponent;
                OnElementAdd(component);
            }
        }
        private void OnCollectionReset(IList oldItems)
        {
            foreach (var item in oldItems)
            {
                var component = item as LayoutComponent;
                OnElementRemove(component);
            }
        }

        private void OnElementAdd(LayoutComponent component)
        {
            if (Children.Count(child => child.Id == component.Id) > 1)
                throw new InvalidOperationException("Collection can not contain duplicate entries");
            component.Parent = this;
        }
        private void OnElementRemove(LayoutComponent item)
        {
            item.Parent = null;
        }
    }
}