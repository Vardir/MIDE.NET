using System;
using System.Globalization;

namespace MIDE.WPF.ValueConverters
{
    public class GridIndexConverter : BaseValueConverter<GridIndexConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int index)
                return index * 2;
            throw new ArgumentException("Not an int value given");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int index)
                return index / 2;
            throw new ArgumentException("Not an int value given");
        }
    }
}