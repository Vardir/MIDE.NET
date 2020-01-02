using System;
using System.Diagnostics;

using Vardirsoft.XApp.API;

namespace Vardirsoft.XApp.Converters
{
    public class ValueConverter<T, Y> : IValueConverter
    {
        private readonly Func<T, Y> _convert;
        private readonly Func<Y, T> _convertBack;

        public ValueConverter(Func<T, Y> convert)
        {
            _convertBack = null;
            _convert = convert ?? throw new ArgumentNullException(nameof(convert));
        }
        public ValueConverter(Func<Y, T> convertBack)
        {
            _convert = null;
            _convertBack = convertBack ?? throw new ArgumentNullException(nameof(convertBack));
        }
        public ValueConverter(Func<T, Y> convert, Func<Y, T> convertBack)
        {
            _convert = convert ?? throw new ArgumentNullException(nameof(convert));
            _convertBack = convertBack ?? throw new ArgumentNullException(nameof(convertBack));
        }

        public object Convert(object value)
        {
            if (value is T tValue)
                return _convert(tValue);

            throw new ArgumentException($"The value was expected to be of type [{typeof(T)}]");
        }
        
        public object ConvertBack(object value)
        {
            if (value is Y yValue)
                return _convertBack(yValue);
                
            throw new ArgumentException($"The value was expected to be of type [{typeof(Y)}]");
        }
        
        [DebuggerStepThrough]
        public Y Convert(T value) => _convert(value);
        
        [DebuggerStepThrough]
        public T ConvertBack(Y value) => _convertBack(value);
    }
}