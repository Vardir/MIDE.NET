using System;
using System.Windows;
using System.Globalization;

namespace MIDE.WPF.ValueConverters
{
    public class IntegerToVisibility : BaseValueConverter<IntegerToVisibility>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int iv)
                return iv == 0 ? Visibility.Collapsed : Visibility.Visible;
            throw new ArgumentException("Not an integer value given");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}