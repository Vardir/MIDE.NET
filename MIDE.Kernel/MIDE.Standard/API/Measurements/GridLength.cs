using System;
using MIDE.Standard.Helpers;

namespace MIDE.Standard.API.Measurements
{
    /// <summary>
    /// <para>A class to represent the </para>
    /// <para>Expected formats: x% -- percentage, x* -- relative, x -- points</para>
    /// </summary>
    public struct GridLength
    {
        public static GridLength Auto => new GridLength(double.NaN, GridLengthType.Auto);

        public readonly double Value;
        public readonly GridLengthType Type;

        public GridLength(string value)
        {
            var result = ParseString(value);
            Value = result.value;
            Type = result.type;
        }
        public GridLength(double value, GridLengthType type)
        {
            Value = value;
            Type = type;
        }        

        /// <summary>
        /// Parses the given string and returns the parsed value
        /// </summary>
        /// <param name="value">The value to parse</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        public static GridLength Parse(string value)
        {
            var result = ParseString(value);
            return new GridLength(result.value, result.type);
        }

        /// <summary>
        /// Parses the given string and returns the parsed value ignoring exceptions
        /// </summary>
        /// <param name="value">The value to parse</param>
        /// <returns></returns>
        public static (bool success, GridLength length) TryParse(string value)
        {
            double length;
            GridLengthType type;
            try
            {
                var result = ParseString(value);
                length = result.value;
                type = result.type;
            }
            catch
            {
                return (false, new GridLength(0.0, GridLengthType.None));
            }
            return (true, new GridLength(length, type));
        }

        public static bool operator==(GridLength left, GridLength right)
        {
            return left.Type == right.Type && left.Value == right.Value;
        }
        public static bool operator !=(GridLength left, GridLength right)
        {
            return left.Type != right.Type || left.Value != right.Value;
        }

        /// <summary>
        /// Parses the given string and returns the parsed value
        /// </summary>
        /// <param name="value">The value to parse</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        private static (double value, GridLengthType type) ParseString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("String can not be null, empty and whitespace", nameof(value));

            char lastChar = value[value.Length - 1];
            if (lastChar == '%' && value.Length > 1)
                return (double.Parse(value.Clamp(value.Length - 1)), GridLengthType.Percentage);
            if (lastChar == '*')
            {
                if (value.Length == 1)
                    return (1.0, GridLengthType.Relative);
                return (double.Parse(value.Clamp(value.Length - 1)), GridLengthType.Relative);
            }
            return (double.Parse(value), GridLengthType.Points);
        }
    }

    public enum GridLengthType
    {
        None,
        Auto,
        Percentage,
        Relative,
        Points
    }
}