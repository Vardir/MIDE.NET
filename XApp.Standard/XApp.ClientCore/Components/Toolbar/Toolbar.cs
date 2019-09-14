using System;
using System.Linq;
using System.Collections.ObjectModel;

using XApp.Helpers;

namespace XApp.Components
{
    public class Toolbar : LayoutComponent
    {
        protected ObservableCollection<IToolBarItem> mItems;

        public ReadOnlyObservableCollection<IToolBarItem> Items { get; }

        public Toolbar(string id) : base(id)
        {
            mItems = new ObservableCollection<IToolBarItem>();
            Items = new ReadOnlyObservableCollection<IToolBarItem>(mItems);
        }

        public bool Contains(string id) => Items.FirstOrDefault(item => item.Id == id) != null;
        public LayoutComponent Find(string id)
        {
            var item = Items.FirstOrDefault(i => i.Id == id);
            if (item == null)
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
                int index = Items.LastIndexWith(i => i.Group == item.Group && i.Order < item.Order);
                if (index > -1)
                {
                    mItems.Insert(index, item);
                }
                else
                {
                    mItems.Add(item);
                }
            }
            else
            {
                throw new ArgumentException("The given component is not toolbar item", nameof(component));
            }
        }
        protected void RemoveChild_Impl(string id)
        {
            var index = Items.FirstIndexWith(i => i.Id == id);
            if (index > -1)
            {  
                mItems.RemoveAt(index);
            }
        }
        protected void RemoveChild_Impl(LayoutComponent component)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));

            if (component is IToolBarItem item)
            {
                mItems.Remove(item);
            }
            else
            {
                throw new ArgumentException($"Toolbar does not contain items of type [{component.GetType()}]");
            }
        }
    }
}