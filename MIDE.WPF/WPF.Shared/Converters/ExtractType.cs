using System;
using System.Globalization;

namespace MIDE.WPF.ValueConverters
{
    public class ExtractType : BaseValueConverter<ExtractType>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "Null";
            string name = value.GetType().Name;
            return name;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}