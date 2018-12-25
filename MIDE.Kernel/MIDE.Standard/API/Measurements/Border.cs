using System;
using MIDE.Standard.Helpers;

namespace MIDE.API.Measurements
{
    /// <summary>
    /// A class to represent a simple box border that can be used to outline the layout components.
    /// <para>Expected formats: x -- all sides, s x -- style and all sides, 
    /// h v -- horizontally and vertically, s h v -- style, horizontally and vertically,
    /// l t r b -- left, top, right and bottom, s l t r b -- style, left, top, right and bottom</para>
    /// </summary>
    public struct Border : IEquatable<Border>
    {
        public readonly double Left;
        public readonly double Top;
        public readonly double Right;
        public readonly double Bottom;
        public readonly BorderStyle Style;

        public Border(double all, BorderStyle style = BorderStyle.Solid)
        {
            Left = all;
            Top = all;
            Right = all;
            Bottom = all;
            Style = style;
        }
        public Border(double horizontal, double vertical, BorderStyle style = BorderStyle.Solid)
        {
            Left = horizontal;
            Top = vertical;
            Right = horizontal;
            Bottom = vertical;
            Style = style;
        }
        public Border(double left, double top, double right, double bottom, BorderStyle style = BorderStyle.Solid)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
            Style = style;
        }
        public Border(string str)
        {
            Border result = Parse(str);
            Left = result.Left;
            Top = result.Top;
            Right = result.Right;
            Bottom = result.Bottom;
            Style = result.Style;
        }

        public override bool Equals(object obj) => obj is Border b && Equals(b);
        public bool Equals(Border other)
        {
            return Left == other.Left &&
                   Top == other.Top &&
                   Right == other.Right &&
                   Bottom == other.Bottom &&
                   Style == other.Style;
        }
        public override int GetHashCode()
        {
            var hashCode = 1645;
            hashCode *= -15211 + Left.GetHashCode();
            hashCode *= -15211 + Top.GetHashCode();
            hashCode *= -15211 + Right.GetHashCode();
            hashCode *= -15223 + Bottom.GetHashCode();
            hashCode *= -14456 + Style.GetHashCode();
            return hashCode & 0xFFF;
        }

        public static bool operator ==(Border b1, Border b2)
        {
            return b1.Style == b2.Style && b1.Left == b2.Left && b1.Right == b2.Right && b1.Top == b2.Top && b1.Bottom == b2.Bottom;
        }
        public static bool operator !=(Border b1, Border b2)
        {
            return b1.Style != b2.Style || b1.Left != b2.Left || b1.Right != b2.Right || b1.Top != b2.Top || b1.Bottom != b2.Bottom;
        }
        public static Border Parse(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentException("String can not be null or empty");
            string[] splited = str.Split(' ');
            if (splited.Length == 1)
            {
                return new Border(double.Parse(splited[0]));
            }
            else if (splited.Length == 2)
            {
                if (EnumHelpers.HasKey(splited[0], out BorderStyle key))
                {
                    return new Border(double.Parse(splited[1]), key);
                }
                double horizontal = double.Parse(splited[0]);
                double vertical = double.Parse(splited[1]);
                return new Border(horizontal, vertical);
            }
            else if (splited.Length == 3)
            {
                if (EnumHelpers.HasKey(splited[0], out BorderStyle key))
                {
                    double horizontal = double.Parse(splited[1]);
                    double vertical = double.Parse(splited[2]);
                    return new Border(horizontal, vertical, key);
                }
            }
            else if (splited.Length == 4)
            {
                double left = double.Parse(splited[0]);
                double top = double.Parse(splited[1]);
                double right = double.Parse(splited[2]);
                double bottom = double.Parse(splited[3]);
                return new Border(left, top, right, bottom);
            }
            else if (splited.Length == 5)
            {
                if (EnumHelpers.HasKey(splited[0], out BorderStyle key))
                {
                    double left = double.Parse(splited[1]);
                    double top = double.Parse(splited[2]);
                    double right = double.Parse(splited[3]);
                    double bottom = double.Parse(splited[4]);
                    return new Border(left, top, right, bottom, key);
                }
            }
            throw new FormatException("The input string has invalid format");
        }
        public static (bool, Border) TryParse(string str)
        {
            Border result;
            try { result = Parse(str); }
            catch
            {
                return (false, new Border());
            }
            return (true, result);
        }
    }

    public enum BorderStyle
    {
        Dotted, Solid, Dash, DashDot, SolidDouble, DashedDouble
    }
}