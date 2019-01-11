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
        private int selectedIndex;
        private Tab selectedTab;

        public Tab SelectedTab
        {
            get => selectedTab;
            private set
            {
                if (value == selectedTab)
                    return;
                selectedTab = value;
                OnPropertyChanged(nameof(SelectedTab));
            }
        }
        public int SelectedIndex
        {
            get => selectedIndex;
            set
            {
                if (value == selectedIndex)
                    return;
                if (Tabs.OutOfRange(value))
                    return;
                selectedIndex = value;
                SelectedTab = Tabs[value];
                OnPropertyChanged(nameof(SelectedIndex));
            }
        }
        public ObservableCollection<Tab> Tabs { get; }

        public TabSection(string id) : base(id)
        {
            Tabs = new ObservableCollection<Tab>();
        }

        public void Insert(int index, Tab tab)
        {
            if (IsSealed)
                throw new InvalidOperationException("Can not add any child elements into a sealed control");
            if (Tabs.OutOfRange(index))
                throw new IndexOutOfRangeException();
            if (tab == null)
                throw new ArgumentNullException(nameof(tab));

            Tabs.Insert(index, tab);
            SelectedIndex = index;
        }
        
        public override bool Contains(string id) => Tabs.IndexOf(t => t.Id == id) > -1;
        public override LayoutComponent Find(string id)
        {
            int index = Tabs.IndexOf(t => t.Id == id);
            if (index == -1)
                return null;
            return Tabs[index];
        }

        protected override void AddChild_Impl(LayoutComponent component)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));
            if (!(component is Tab tab))
                throw new ArgumentException($"Argument was expected to be a Tab, but {component.GetType()} given");
            if (Tabs.Contains(t => t.Id == tab.Id))
                throw new ArgumentException($"The section already contains a tab with ID {tab.Id}");
            Tabs.Add(tab);
            SelectedIndex = Tabs.Count - 1;
        }
        protected override void RemoveChild_Impl(string id)
        {
            int index = Tabs.IndexOf(t => t.Id == id);
            if (index == -1)
                return;
            Tabs.RemoveAt(index);
            if (index == selectedIndex)
                SelectedIndex = 0;
        }
        protected override void RemoveChild_Impl(LayoutComponent component)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));
            if (!(component is Tab tab))
                throw new ArgumentException($"The tab section component does not contain elements of type {component.GetType()}");
            int index = Tabs.IndexOf(t => t.Id == tab.Id);
            if (index == -1)
                return;
            Tabs.RemoveAt(index);
            if (index == selectedIndex)
                SelectedIndex = 0;
        }
    }
}