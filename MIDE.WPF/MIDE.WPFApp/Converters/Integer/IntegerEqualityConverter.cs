using System;
using System.Windows;
using System.Globalization;

namespace MIDE.WPFApp.ValueConverters
{
    public class IntegerEqualityConverter : BaseMultiValueConverter<IntegerEqualityConverter>
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length == 2 &&
                values[0] is int value1 && values[1] is int value2)
            {
                return value1 == value2;
            }

            return DependencyProperty.UnsetValue;
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}