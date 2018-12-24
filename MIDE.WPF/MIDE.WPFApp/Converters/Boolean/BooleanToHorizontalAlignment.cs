using System;
using System.Windows;
using System.Globalization;

namespace MIDE.WPFApp.ValueConverters.ValueConverters
{
    /// <summary>
    /// If the value if true, return alignment to the right, otherwise return to the left
    /// </summary>
    public class BooleanToHorizontalAlignment : BaseValueConverter<BooleanToHorizontalAlignment>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null) return (bool)value ? HorizontalAlignment.Right : HorizontalAlignment.Left;
            return (bool)value ? HorizontalAlignment.Left : HorizontalAlignment.Right;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}