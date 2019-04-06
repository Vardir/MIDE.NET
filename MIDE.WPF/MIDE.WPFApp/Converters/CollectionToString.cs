using System;
using System.Collections;
using MIDE.WPFApp.Helpers;
using System.Globalization;

namespace MIDE.WPFApp.ValueConverters
{
    public class CollectionToString : BaseValueConverter<CollectionToString>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable collection)
            {
                return string.Join("\n", collection.Transform(obj => obj.ToString()));
            }
            throw new ArgumentException("Not a collection value given");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}