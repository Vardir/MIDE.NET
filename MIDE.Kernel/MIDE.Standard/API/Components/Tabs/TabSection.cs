using System;
using System.Linq;
using MIDE.Helpers;
using System.Collections.ObjectModel;

namespace MIDE.API.Components
{
    /// <summary>
    /// A container for the group of tabs
    /// </summary>
    public class TabSection : LayoutContainer
    {
        public ObservableCollection<Tab> Tabs { get; }

        public TabSection(string id) : base(id)
        {
            Tabs = new ObservableCollection<Tab>();
        }

        public override void AddChild(LayoutComponent component)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));
            if (!(component is Tab tab))
                throw new ArgumentException($"Argument was expected to be a Tab, but {component.GetType()} given");
            if (Tabs.Contains(t => t.Id == tab.Id))
                throw new ArgumentException($"The section already contains a tab with ID {tab.Id}");
            Tabs.Add(tab);
        }
        public void Insert(int index, Tab tab)
        {
            if (Tabs.OutOfRange(index))
                throw new IndexOutOfRangeException();
            if (tab == null)
                throw new ArgumentNullException(nameof(tab));
            Tabs.Insert(index, tab);
        }
        public override void RemoveChild(string id)
        {
            int index = Tabs.IndexOf(t => t.Id == id);
            if (index == -1)
                return;
            Tabs.RemoveAt(index);
        }
        public override void RemoveChild(LayoutComponent component)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));
            if (!(component is Tab tab))
                throw new ArgumentException($"The tab section component does not contain elements of type {component.GetType()}");
            Tabs.Remove(tab);
        }

        public override bool Contains(string id) => Tabs.IndexOf(t => t.Id == id) > -1;
        public override LayoutComponent Find(string id)
        {
            int index = Tabs.IndexOf(t => t.Id == id);
            if (index == -1)
                return null;
            return Tabs[index];
        }
    }
}