using System;
using System.Linq;
using System.Collections.ObjectModel;

using Vardirsoft.Shared.Helpers;

namespace Vardirsoft.XApp.Components
{
    public class Toolbar : LayoutComponent
    {
        protected readonly ObservableCollection<IToolBarItem> _items;

        public ReadOnlyObservableCollection<IToolBarItem> Items { get; }

        public Toolbar(string id) : base(id)
        {
            _items = new ObservableCollection<IToolBarItem>();
            Items = new ReadOnlyObservableCollection<IToolBarItem>(_items);
        }

        public bool Contains(string id) => Items.FirstOrDefault(item => item.Id == id).HasValue();
        public LayoutComponent Find(string id)
        {
            var item = Items.FirstOrDefault(i => i.Id == id);
            if (item is null)
                return null;

            return item as LayoutComponent;
        }

        protected override LayoutComponent CloneInternal(string id)
        {
            var clone = new Toolbar(id);
            clone.Items.AddRange(Items.Select(item => item.Clone() as IToolBarItem));

            return clone;
        }

        protected void AddChild_Impl(LayoutComponent component)
        {
            if (component is IToolBarItem item)
            {
                var index = Items.LastIndexWith(i => i.Group == item.Group && i.Order < item.Order);
                if (index > -1)
                {
                    _items.Insert(index, item);
                }
                else
                {
                    _items.Add(item);
                }
            }
            else
            {
                throw new ArgumentException("The given component is not toolbar item", nameof(component));
            }
        }
        protected void RemoveChild_Impl(string id)
        {
            var index = Items.IndexWith(i => i.Id == id);
            if (index > -1)
            {  
                _items.RemoveAt(index);
            }
        }
        protected void RemoveChild_Impl(LayoutComponent component)
        {
            if (component is null)
                throw new ArgumentNullException(nameof(component));

            if (component is IToolBarItem item)
            {
                _items.Remove(item);
            }
            else
            {
                throw new ArgumentException($"Toolbar does not contain items of type [{component.GetType()}]");
            }
        }
    }
}