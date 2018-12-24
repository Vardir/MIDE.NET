using System;
using System.Globalization;

namespace MIDE.WPFApp.ValueConverters
{
    public class EnumToInteger : BaseValueConverter<EnumToInteger>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum en)
                return System.Convert.ToInt32(en);
            throw new ArgumentException("Not an enum value given");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}