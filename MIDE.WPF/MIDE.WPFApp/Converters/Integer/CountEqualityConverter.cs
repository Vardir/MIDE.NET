using System;
using System.Windows;
using System.Globalization;

namespace MIDE.WPFApp.ValueConverters
{
    public class CountEqualityConverter : BaseMultiValueConverter<CountEqualityConverter>
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length == 2 &&
                values[0] is int index && values[1] is int len)
            {
                return index == (len + 1);
            }

            return DependencyProperty.UnsetValue;
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}