using System;
using System.Windows;
using WPF.Extensibility;
using System.Collections.Generic;

namespace Terminal.WPF
{
    public class TerminalUIExtension : UIExtension
    {
        public TerminalUIExtension() : base("MIDE.Terminal.UI")
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