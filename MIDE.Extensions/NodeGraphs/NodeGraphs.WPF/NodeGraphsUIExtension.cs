using System;
using System.Windows;
using WPF.Extensibility;
using System.Collections.Generic;

namespace NodeGraphs.WPF
{
    public class NodeGraphsUIExtension : UIExtension
    {
        public NodeGraphsUIExtension() : base("MIDE.NodeGraphs.UI")
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