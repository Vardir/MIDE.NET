using System;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Markup;

namespace MIDE.WPF.ValueConverters
{
    public abstract class BaseValueConverter<T> : MarkupExtension, IValueConverter
        where T: class, new()
    {
        static T converter = null;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return converter ?? (converter = new T());
        }

        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);
        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
    }
}