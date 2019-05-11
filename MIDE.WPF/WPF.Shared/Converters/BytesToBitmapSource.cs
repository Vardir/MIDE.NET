using System;
using System.IO;
using System.Globalization;
using System.Windows.Media.Imaging;

namespace MIDE.WPF.ValueConverters
{
    public class BytesToBitmapSource : BaseValueConverter<BytesToBitmapSource>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte[] array)
            {
                BitmapImage image = new BitmapImage();
                using (MemoryStream ms = new MemoryStream(array))
                {
                    ms.Position = 0;
                    image.BeginInit();
                    image.StreamSource = ms;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                }
                return image;
            }
            throw new ArgumentException("Not a byte array value given");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}