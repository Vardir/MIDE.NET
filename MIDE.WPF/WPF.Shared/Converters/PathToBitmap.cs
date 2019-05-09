using System;
using System.IO;
using System.Globalization;
using System.Windows.Media.Imaging;

namespace MIDE.WPF.ValueConverters
{
    public class PathToBitmap : BaseValueConverter<PathToBitmap>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string path)
            {
                if (!File.Exists(path))
                    throw new FileNotFoundException("File does not exist", path);
                string fullPath = Path.GetFullPath(path);
                return new BitmapImage(new Uri(fullPath));
            }
            throw new ArgumentException("Not a string value given");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}