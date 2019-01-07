using System;
using MIDE.API.Services;
using MIDE.API.Components;
using System.Collections.Generic;

namespace MIDE.WPFApp.Services
{
    public class UIManager : IUIManager
    {
        private Dictionary<string, TabSection> sections;

        public Menu ApplicationMenu { get; }

        public TabSection this[string id] => sections[id];
        
        public UIManager()
        {
            sections = new Dictionary<string, TabSection>();
            ApplicationMenu = new Menu("app-menu");
        }

        public void AddSection(TabSection section)
        {
            if (section == null)
                throw new ArgumentNullException(nameof(section));
            if (sections.ContainsKey(section.Id))
                return;
            sections.Add(section.Id, section);
        }
    }
}