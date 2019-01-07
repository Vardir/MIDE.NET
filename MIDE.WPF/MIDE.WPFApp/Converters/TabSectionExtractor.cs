using System;
using MIDE.WPFApp.Services;
using System.Globalization;

namespace MIDE.WPFApp.ValueConverters
{
    public class TabSectionExtractor : BaseValueConverter<TabSectionExtractor>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is UIManager manager)
                return manager[parameter as string].Tabs;
            throw new ArgumentException("Not a UIManager value given");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}