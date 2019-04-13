using System;
using System.Globalization;
using System.Windows.Controls;

namespace MIDE.WPFApp.ValueConverters
{
    public class BoolToMultipleSelectionMode : BaseValueConverter<BoolToMultipleSelectionMode>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return b ? SelectionMode.Extended : SelectionMode.Single;
            throw new ArgumentException("Not a boolean value given");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}