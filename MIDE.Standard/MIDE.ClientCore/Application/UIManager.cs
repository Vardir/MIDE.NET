using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using MIDE.Components;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace MIDE.Application
{
    public abstract class UIManager
    {
        private readonly List<TabSection> tabSections;
        private readonly Dictionary<string, Tab> tabs;

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
        public abstract void RegisterUIExtension(object obj);
        public abstract void RegisterUIExtension(string path);
        public abstract void RegisterUIExtension(Type type);
        public abstract void RegisterUIExtension(Assembly assembly);

        public (DialogResult dialogResult, T data) OpenDialog<T>(BaseDialogBox<T> dialogBox)
        {
            OpenDialog_Impl(dialogBox);
            return (dialogBox.SelectedResult, dialogBox.GetData());
        }

        public T GetTab<T>()
            where T: Tab
        {
            foreach (var kvp in tabs)
            {
                if (kvp.Value is T)
                    return kvp.Value as T;
            }
            return null;
        }
        public T GetOrAddTab<T>(string section, Func<T> generator)
            where T: Tab
        {
            foreach (var kvp in tabs)
            {
                if (kvp.Value is T)
                    return kvp.Value as T;
            }
            var tab = generator();
            this[section].AddChild(tab);
            return tab;
        }
        public IEnumerable<T> GetTabs<T>()
            where T: Tab
        {
            foreach (var kvp in tabs)
            {
                if (kvp.Value is T)
                    yield return kvp.Value as T;
            }
        }

        protected abstract (DialogResult result, T value) OpenDialog_Impl<T>(BaseDialogBox<T> dialogBox);

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
    }
}