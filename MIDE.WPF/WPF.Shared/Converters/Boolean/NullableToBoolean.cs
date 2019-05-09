using System;
using System.Globalization;

namespace MIDE.WPF.ValueConverters
{
    public class NullableToBoolean : BaseValueConverter<NullableToBoolean>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}