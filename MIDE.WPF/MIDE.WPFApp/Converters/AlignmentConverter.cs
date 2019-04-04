using System;
using System.Globalization;
using MC = MIDE.API.Components;
using SW = System.Windows;

namespace MIDE.WPFApp.ValueConverters
{
    public class AlignmentConverter : BaseValueConverter<AlignmentConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MC.HorizontalAlignment horiz)
            {
                return horiz switch
                {
                    MC.HorizontalAlignment.Center => SW.HorizontalAlignment.Center,
                    MC.HorizontalAlignment.Left => SW.HorizontalAlignment.Left,
                    MC.HorizontalAlignment.Right => SW.HorizontalAlignment.Right,
                    MC.HorizontalAlignment.Stretch => SW.HorizontalAlignment.Stretch,
                    _ => throw new NotImplementedException()
                };
            }
            if (value is MC.VerticalAlignment vert)
            {
                return vert switch
                {
                    MC.VerticalAlignment.Bottom => SW.VerticalAlignment.Bottom,
                    MC.VerticalAlignment.Top => SW.VerticalAlignment.Top,
                    MC.VerticalAlignment.Center => SW.VerticalAlignment.Center,
                    MC.VerticalAlignment.Stretch => SW.VerticalAlignment.Stretch,
                    _ => throw new NotImplementedException()
                };
            }
            return null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}