using System;
using System.Windows;
using WPF.Extensibility;
using System.Collections.Generic;

namespace GanttChart.WPF
{
    public class GanttChartUIExtension : UIExtension
    {
        public GanttChartUIExtension() : base("GanttChart.WPF")
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