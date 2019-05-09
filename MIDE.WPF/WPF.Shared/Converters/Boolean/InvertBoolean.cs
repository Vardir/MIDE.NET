using System;
using System.Globalization;

namespace MIDE.WPF.ValueConverters.ValueConverters
{
    public class InvertBoolean : BaseValueConverter<InvertBoolean>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new ArgumentException("Not a boolean value given!");
            return !(bool)value;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}