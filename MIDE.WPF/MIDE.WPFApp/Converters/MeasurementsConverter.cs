using System;
using MIDE.WPFApp.Helpers;
using System.Globalization;
using MIDE.API.Measurements;

namespace MIDE.WPFApp.ValueConverters
{
    public class MeasurementsConverter : BaseValueConverter<MeasurementsConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case GridLength len: return len.ToWindows();
                case BoundingBox box: return box.ToWindows();
                default:
                    throw new InvalidCastException();
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}