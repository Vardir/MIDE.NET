using System;
using System.Windows;
using System.Globalization;

namespace MIDE.WPF.ValueConverters
{
    public class NullableToVisibility : BaseValueConverter<NullableToVisibility>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? Visibility.Visible : Visibility.Collapsed;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}