using System;
using System.Linq;
using MIDE.Helpers;
using System.Collections.ObjectModel;

namespace MIDE.API.Components
{
    public class Toolbar : LayoutContainer
    {
        public ObservableCollection<IToolBarItem> Items { get; }

        public Toolbar(string id) : base(id)
        {
            Items = new ObservableCollection<IToolBarItem>();
        }

        public override bool Contains(string id) => Items.FirstOrDefault(item => item.Id == id) != null;
        public override LayoutComponent Find(string id)
        {
            var item = Items.FirstOrDefault(i => i.Id == id);
            if (item == null)
                return null;
            return item as LayoutComponent;
        }

        protected override void AddChild_Impl(LayoutComponent component)
        {
            if (!(component is IToolBarItem item))
                throw new ArgumentException("The given component is not toolbar item", nameof(component));

            int index = Items.LastIndexWith(i => i.Group == item.Group && i.Order < item.Order);
            if (index == -1)
            {
                Items.Add(item);
                return;
            }
            Items.Insert(index, item);
        }
        protected override void RemoveChild_Impl(string id)
        {
            int index = Items.FirstIndexWith(i => i.Id == id);
            if (index == -1)
                return;
            Items.RemoveAt(index);
        }
        protected override void RemoveChild_Impl(LayoutComponent component)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));
            if (!(component is IToolBarItem item))
                throw new ArgumentException($"Toolbar does not contain items of type [{component.GetType()}]");
            Items.Remove(item);
        }
    }
}