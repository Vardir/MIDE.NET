using System.Windows;

namespace MIDE.WPFApp.Helpers
{
    public static class ResourceDictionaryHelpers
    {
        public static void Add(this ResourceDictionary dictionary, ResourceDictionary source)
        {
            foreach (var key in source.Keys)
            {
                dictionary.Add(key, source[key]);
            }
        }
        public static void Update(this ResourceDictionary dictionary, ResourceDictionary source)
        {
            foreach (var key in source.Keys)
            {
                var prev = dictionary[key];
                var next = source[key];
                dictionary[key] = source[key];
            }
        }
    }
}