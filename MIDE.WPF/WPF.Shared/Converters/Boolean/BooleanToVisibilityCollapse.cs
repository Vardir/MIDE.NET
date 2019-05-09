using System;
using System.Windows;
using System.Globalization;

namespace MIDE.WPF.ValueConverters.ValueConverters
{
    public class BooleanToVisibilityCollapse : BaseValueConverter<BooleanToVisibilityCollapse>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
                return (bool)value ? Visibility.Visible : Visibility.Collapsed;
            return (bool)value ? Visibility.Collapsed : Visibility.Visible;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}