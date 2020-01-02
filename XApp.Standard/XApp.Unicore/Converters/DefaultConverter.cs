using System.Diagnostics;

using Vardirsoft.XApp.API;

namespace Vardirsoft.XApp.Converters
{
    public class DefaultConverter : IValueConverter
    {
        [DebuggerStepThrough]
        public object Convert(object value) => value;
        
        [DebuggerStepThrough]
        public object ConvertBack(object value) => value;
    }
}