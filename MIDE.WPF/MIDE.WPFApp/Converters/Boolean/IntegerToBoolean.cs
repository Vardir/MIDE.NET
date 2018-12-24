using System;
using System.Globalization;

namespace MIDE.WPFApp.ValueConverters
{
    public class IntegerToBoolean : BaseValueConverter<IntegerToBoolean>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int i)
                return i != 0;
            return false;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}