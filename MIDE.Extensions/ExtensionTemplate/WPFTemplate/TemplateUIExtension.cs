using System;
using System.Windows;
using WPF.Extensibility;
using System.Collections.Generic;

namespace WPFTemplate
{
    public class TemplateUIExtension : UIExtension
    {
        public TemplateUIExtension() : base("WPFTemplate")
        {

        }

        public override IEnumerable<string> GetResourceDictionariesPaths()
        {
            yield return "Styles/DefaultControls.xaml";
        }
        public override IEnumerable<ResourceDictionary> GetResourceDictionaries()
        {
            yield return new ResourceDictionary()
            {
                Source = new Uri($"pack://application:,,,/{Name};component/Styles/DefaultControls.xaml", UriKind.RelativeOrAbsolute)
            };
        }
    }
}