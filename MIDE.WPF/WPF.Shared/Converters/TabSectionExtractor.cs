using System;
using MIDE.API.Services;
using System.Globalization;

namespace MIDE.WPF.ValueConverters
{
    public class TabSectionExtractor : BaseValueConverter<TabSectionExtractor>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is UIManager manager)
                return manager[parameter as string];
            throw new ArgumentException("Not a UIManager value given");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}