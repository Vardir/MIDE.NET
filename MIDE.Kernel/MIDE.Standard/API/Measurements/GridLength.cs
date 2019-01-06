using System;
using MIDE.Helpers;

namespace MIDE.API.Measurements
{
    /// <summary>
    /// <para>A class to represent the size-measurement properties of the layout components</para>
    /// <para>Expected formats: x% -- percentage, x* -- relative, x -- points</para>
    /// </summary>
    public struct GridLength : IEquatable<GridLength>, IComparable<GridLength>
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

        public bool Equals(GridLength other) => Value == other.Value && Type == other.Type;
        public override bool Equals(object obj) => obj is GridLength gl && Equals(gl);
        public override int GetHashCode()
        {
            var hashCode = 1574897;
            hashCode *= -152295 + Value.GetHashCode();
            hashCode *= -152295 + Type.GetHashCode();
            return hashCode & 0xFFF;
        }
        public int CompareTo(GridLength other)
        {
            if (Type == other.Type)
            {
                if (Value < other.Value)
                    return 1;
                if (Value > other.Value)
                    return 1;
            }
            return 0;
        }
        public override string ToString()
        {
            switch (Type)
            {
                case GridLengthType.Auto: return "auto";
                case GridLengthType.Percentage: return $"{Value}%";
                case GridLengthType.Points: return $"{Value}pt";
                case GridLengthType.Relative: return $"{Value}*";
                case GridLengthType.None:
                default: return $"none:{Value}";
            }
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

        public static bool operator ==(GridLength left, GridLength right)
        {
            return left.Type == right.Type && left.Value == right.Value;
        }
        public static bool operator !=(GridLength left, GridLength right)
        {
            return left.Type != right.Type || left.Value != right.Value;
        }
        public static bool operator <(GridLength left, GridLength right)
        {
            return left.Type == right.Type && left.Value < right.Value;
        }
        public static bool operator >(GridLength left, GridLength right)
        {
            return left.Type == right.Type && left.Value > right.Value;
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
            if (value.Length == 4)
            {
                if (value.Equals("auto", StringComparison.OrdinalIgnoreCase))
                    return (double.NaN, GridLengthType.Auto);
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