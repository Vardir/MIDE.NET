using System;
using System.Globalization;
using WIN = System.Windows;

namespace MIDE.WPF.ValueConverters
{
    public class BooleanVisibilityConverter : BaseValueConverter<BooleanVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return b ? WIN.Visibility.Visible : WIN.Visibility.Collapsed;
            throw new ArgumentException("Not a boolean value given");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}