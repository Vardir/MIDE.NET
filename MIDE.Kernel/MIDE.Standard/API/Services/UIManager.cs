using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using MIDE.API.Components;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace MIDE.API.Services
{
    public class UIManager
    {
        private List<TabSection> tabSections;
        private Dictionary<string, Tab> tabs;

        public Platform CurrentPlatform { get; set; }
        public Menu ApplicationMenu { get; }

        public TabSection this[string id] => tabSections.Find(ts => ts.Id == id);

        public UIManager()
        {
            ApplicationMenu = new Menu("menu");
            tabSections = new List<TabSection>();
            tabs = new Dictionary<string, Tab>();
        }

        public void AddTab(Tab tab, string sectionId)
        {
            if (tab == null)
                throw new ArgumentNullException(nameof(tab));
            if (sectionId == null)
                throw new ArgumentNullException(nameof(sectionId));
            var tabSection = tabSections.FirstOrDefault(ts => ts.Id == sectionId);
            if (tabSection == null)
                throw new ArgumentException($"Section with ID '{sectionId}' does not exist");
            
            tabSection.AddChild(tab);
        }
        public void AddTabSection(TabSection section)
        {
            if (section == null)
                throw new ArgumentNullException(nameof(section));
            if (tabSections.FirstOrDefault(ts => ts.Id == section.Id) != null)
                throw new ArgumentException("Duplicate tab section entry");

            tabSections.Add(section);
            section.Tabs.CollectionChanged += SectionTabs_CollectionChanged;
        }
        public virtual void RegisterUIExtension(object obj) { }
        public virtual void RegisterUIExtension(string path) { }
        public virtual void RegisterUIExtension(Type type) { }
        public virtual void RegisterUIExtension(Assembly assembly) { }

        private void SectionTabs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    OnSectionAddTabs(e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    OnSectionRemoveTabs(e.OldItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    OnSectionReplaceTabs(e.NewItems, e.OldItems);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    tabs.Clear();
                    break;
            }
        }

        public Tab GetTab<T>()
            where T: Tab
        {
            foreach (var kvp in tabs)
            {
                if (kvp.Value is T)
                    return kvp.Value;
            }
            return null;
        }
        public IEnumerable<Tab> GetTabs<T>()
            where T: Tab
        {
            foreach (var kvp in tabs)
            {
                if (kvp.Value is T)
                    yield return kvp.Value;
            }
        }

        private void OnSectionAddTabs(IList items)
        {
            foreach (var item in items)
            {
                var tab = item as Tab;
                if (tabs.ContainsKey(tab.Id))
                {
                    if (!tabs[tab.Id].AllowDuplicates)
                        throw new ArgumentException("Duplicate tab entry");
                    tab.GenerateNextId();
                }
                tabs.Add(tab.Id, tab);
            }
        }
        private void OnSectionRemoveTabs(IList items)
        {
            foreach (var item in items)
            {
                var tab = item as Tab;
                if (tabs.ContainsKey(tab.Id))
                    tabs.Remove(tab.Id);
            }
        }
        private void OnSectionReplaceTabs(IList newItems, IList oldItems)
        {
            OnSectionRemoveTabs(oldItems);
            OnSectionAddTabs(newItems);
        }
    }

    public enum Platform
    {
        WindowsConsole, WPF, WinForms //TODO: add more platforms
    }
}