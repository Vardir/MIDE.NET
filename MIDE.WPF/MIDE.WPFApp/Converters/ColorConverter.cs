using System;
using SD = System.Drawing;
using System.Globalization;
using SWM = System.Windows.Media;

namespace MIDE.WPFApp.ValueConverters
{
    public class ColorConverter : BaseValueConverter<ColorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SD.Color color)
            {
                return new SWM.Color()
                {
                    A = color.A,
                    R = color.R,
                    G = color.G,
                    B = color.B
                };
            }
            if (value is SWM.Color colorW)
                return SD.Color.FromArgb(colorW.A, colorW.R, colorW.G, colorW.B);
            throw new ArgumentException("Not a color value given");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}