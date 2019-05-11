using System.Windows;
using MIDE.API.Visuals;
using System.Windows.Controls;

namespace MIDE.WPF.Styles
{
    public class GlyphTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FontAwesomeTemplate { get; set; }
        public DataTemplate ImagePathTemplate { get; set; }
        public DataTemplate BytesMapTemplate { get; set; }
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
                    case GlyphKind.ImagePath:
                        return ImagePathTemplate;
                    case GlyphKind.ByteArray:
                        return BytesMapTemplate;
                }
            }
            return base.SelectTemplate(item, container);
        }
    }
}