using System;
using System.Windows;
using System.Globalization;

namespace MIDE.WPFApp.ValueConverters.ValueConverters
{
    public class BooleanToVisibility : BaseValueConverter<BooleanToVisibility>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
                return (bool)value ? Visibility.Hidden : Visibility.Visible;
            return (bool)value ? Visibility.Visible : Visibility.Hidden;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is Visibility)) return false;
            return (Visibility)value == Visibility.Visible;
        }
    }
}