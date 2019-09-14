using XApp.API;

namespace XApp.Converters
{
    public class DefaultConverter : IValueConverter
    {
        public object Convert(object value) => value;
        public object ConvertBack(object value) => value;
    }
}