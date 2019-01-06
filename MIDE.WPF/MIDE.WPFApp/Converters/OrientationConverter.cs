using System;
using MIDE.API.Components;
using System.Globalization;
using System.Windows.Controls;

namespace MIDE.WPFApp.ValueConverters
{
    public class OrientationConverter : BaseValueConverter<OrientationConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Orientation orientation)
            {
                switch (orientation)
                {
                    case Orientation.Horizontal: return StackOrientation.Horizontal;
                    case Orientation.Vertical: return StackOrientation.Vertical;
                }
            }
            else if (value is StackOrientation stackOrientation)
            {
                switch (stackOrientation)
                {
                    case StackOrientation.Horizontal: return Orientation.Horizontal;
                    case StackOrientation.Vertical: return Orientation.Vertical;
                }
            }
            throw new InvalidCastException();
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}