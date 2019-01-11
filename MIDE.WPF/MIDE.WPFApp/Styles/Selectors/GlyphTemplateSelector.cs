using System.Windows;
using MIDE.API.Visuals;
using System.Windows.Controls;

namespace MIDE.WPFApp.Styles
{
    public class GlyphTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FontAwesomeTemplate { get; set; }
        public DataTemplate BitmapTemplate { get; set; }
        //public DataTemplate ImagePathTemplate { get; set; }
        //public DataTemplate SymbolicTemplate { get; set; }
        public DataTemplate StringTemplate { get; set; }

        public GlyphTemplateSelector() { }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Glyph glyph)
            {
                switch (glyph.Kind)
                {
                    case GlyphKind.FontAwesome:
                        return FontAwesomeTemplate;
                        //TODO: add cases
                }
            }
            return base.SelectTemplate(item, container);
        }
    }
}