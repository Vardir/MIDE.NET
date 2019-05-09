using System;
using System.Linq;
using MIDE.Helpers;
using MIDE.API.Commands;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

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
                if (value >= Tabs.Count || value < -1)
                    return;
                selectedIndex = value;
                if (selectedIndex >= 0)
                    SelectedTab = Tabs[value];
                else
                    SelectedTab = null;
                OnPropertyChanged(nameof(SelectedIndex));
            }
        }
        public ObservableCollection<Tab> Tabs { get; }

        public TabSection(string id) : base(id)
        {
            Tabs = new ObservableCollection<Tab>();
            Tabs.CollectionChanged += Tabs_CollectionChanged;
        }

        public void Insert(int index, Tab tab)
        {
            if (IsSealed)
                throw new InvalidOperationException("Can not add any child elements into a sealed control");
            if (Tabs.OutOfRange(index))
                throw new IndexOutOfRangeException();
            if (tab == null)
                throw new ArgumentNullException(nameof(tab));
            if (Tabs.Contains(t => t.Id == tab.Id))
                throw new ArgumentException($"The section already contains a tab with ID {tab.Id}");

            Tabs.Insert(index, tab);
            SelectedIndex = index;
        }

        public override bool Contains(string id) => Tabs.Find(t => t.Id == id) != null;
        public override LayoutComponent Find(string id) => Tabs.Find(t => t.Id == id);

        protected override LayoutComponent CloneInternal(string id)
        {
            TabSection clone = new TabSection(id);
            clone.Tabs.AddRange(Tabs.Select(tab => tab.Clone() as Tab));
            return clone;
        }

        protected override void AddChild_Impl(LayoutComponent component)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));
            if (!(component is Tab tab))
                throw new ArgumentException($"Argument was expected to be a Tab, but {component.GetType()} given");
            
            Tabs.Add(tab);
        }
        protected override void RemoveChild_Impl(string id)
        {
            int index = Tabs.IndexOf(t => t.Id == id);
            if (index == -1)
                return;
            Tabs.RemoveAt(index);
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
        }

        private void Tabs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    OnTabsAdd(e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    OnTabsReplace(e.NewItems, e.OldItems);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    OnTabsRemove(e.OldItems);
                    break;
            }
        }

        private void OnTabsAdd(IList items)
        {
            foreach (var item in items)
            {
                var tab = item as Tab;
                if (Tabs.Count(t => t.Id == tab.Id) > 1)
                    throw new ArgumentException($"The section already contains a tab with ID {tab.Id}");
                tab.ParentSection = this;
                tab.Parent = this;
            }
            SelectedIndex = Tabs.Count - 1;
        }
        private void OnTabsRemove(IList items)
        {
            foreach (var item in items)
            {
                var tab = item as Tab;
                tab.ParentSection = null;
                tab.Parent = null;
            }
            SelectedIndex = Tabs.Count - 1;
        }
        private void OnTabsReplace(IList newItems, IList oldItems)
        {
            OnTabsAdd(newItems);
            OnTabsRemove(oldItems);
        }
    }
}