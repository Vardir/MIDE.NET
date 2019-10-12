using Vardirsoft.XApp.API;

namespace Vardirsoft.XApp.Converters
{
    public class DefaultConverter : IValueConverter
    {
        public object Convert(object value) => value;
        public object ConvertBack(object value) => value;
    }
}