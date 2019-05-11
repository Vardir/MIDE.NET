using System.Windows;
using System.Collections.Generic;

namespace WPF.Extensibility
{
    public abstract class UIExtension
    {
        public string Name { get; }

        public UIExtension(string name)
        {
            Name = name;
        }

        public abstract IEnumerable<string> GetResourceDictionariesPaths();
        public abstract IEnumerable<ResourceDictionary> GetResourceDictionaries();
    }
}