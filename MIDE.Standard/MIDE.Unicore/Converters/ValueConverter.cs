using System;
using MIDE.API;

namespace MIDE.Converters
{
    public class ValueConverter<T, Y> : IValueConverter
    {
        private readonly Func<T, Y> convert;
        private readonly Func<Y, T> convertBack;

        public ValueConverter(Func<T, Y> convert)
        {
            convertBack = null;
            this.convert = convert ?? throw new ArgumentNullException(nameof(convert));
        }
        public ValueConverter(Func<Y, T> convertBack)
        {
            convert = null;
            this.convertBack = convertBack ?? throw new ArgumentNullException(nameof(convertBack));
        }
        public ValueConverter(Func<T, Y> convert, Func<Y, T> convertBack)
        {
            this.convert = convert ?? throw new ArgumentNullException(nameof(convert));
            this.convertBack = convertBack ?? throw new ArgumentNullException(nameof(convertBack));
        }

        public object Convert(object value)
        {
            if (value is T _value)
                return convert(_value);
            throw new ArgumentException($"The value was expected to be of type [{typeof(T)}]");
        }
        public object ConvertBack(object value)
        {
            if (value is Y _value)
                return convertBack(_value);
            throw new ArgumentException($"The value was expected to be of type [{typeof(Y)}]");
        }
        public Y Convert(T value) => convert(value);
        public T ConvertBack(Y value) => convertBack(value);
    }
}