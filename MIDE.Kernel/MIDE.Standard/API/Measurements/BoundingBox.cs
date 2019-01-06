using System;
using System.Collections.Generic;

namespace MIDE.API.Measurements
{
    /// <summary>
    /// A class to represent a simple box that can be used as margin or padding box.
    /// <para>Expected formats: x -- all sides, h v -- horizontally and vertically, l t r b -- left, top, right and bottom</para>
    /// </summary>
    public struct BoundingBox : IEquatable<BoundingBox>
    {
        public readonly double Left;
        public readonly double Top;
        public readonly double Right;
        public readonly double Bottom;

        public BoundingBox(double all)
        {
            Left = all;
            Top = all;
            Right = all;
            Bottom = all;
        }
        public BoundingBox(double horizontal, double vertical)
        {
            Left = horizontal;
            Top = vertical;
            Right = horizontal;
            Bottom = vertical;
        }
        public BoundingBox(double left, double top, double right, double bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
        public BoundingBox(string str)
        {
            BoundingBox result = Parse(str);
            Left = result.Left;
            Top = result.Top;
            Right = result.Right;
            Bottom = result.Bottom;
        }

        public bool Equals(BoundingBox other)
        {
            return Left.Equals(other.Left) &&
                   Right.Equals(other.Right) &&
                   Top.Equals(other.Top) &&
                   Bottom.Equals(other.Bottom);
        }
        public override bool Equals(object obj) => obj is BoundingBox b && Equals(b);        
        public override int GetHashCode()
        {
            var hashCode = 55158;
            hashCode *= -152 + EqualityComparer<double>.Default.GetHashCode(Left);
            hashCode *= -152 + EqualityComparer<double>.Default.GetHashCode(Right);
            hashCode *= -152 + EqualityComparer<double>.Default.GetHashCode(Top);
            hashCode *= -152 + EqualityComparer<double>.Default.GetHashCode(Bottom);
            return hashCode & 0xFFF;
        }

        public static bool operator==(BoundingBox b1, BoundingBox b2)
        {
            return b1.Left == b2.Left && b1.Right == b2.Right && b1.Top == b2.Top && b1.Bottom == b2.Bottom;
        }
        public static bool operator !=(BoundingBox b1, BoundingBox b2)
        {
            return b1.Left != b2.Left || b1.Right != b2.Right || b1.Top != b2.Top || b1.Bottom != b2.Bottom;
        }
        public static BoundingBox Parse(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentException("String can not be null or empty");
            string[] splited = str.Split(' ');
            if (splited.Length == 1)
            {
                double length = double.Parse(splited[0]);
                return new BoundingBox(length);
            }
            else if (splited.Length == 2)
            {
                double horizontal = double.Parse(splited[0]);
                double vertical = double.Parse(splited[1]);
                return new BoundingBox(horizontal, vertical);
            }
            else if (splited.Length == 4)
            {
                double left = double.Parse(splited[0]);
                double top = double.Parse(splited[1]);
                double right = double.Parse(splited[2]);
                double bottom = double.Parse(splited[3]);
                return new BoundingBox(left, top, right, bottom);
            }
            throw new FormatException("The input string has invalid format");
        }
        public static (bool, BoundingBox) TryParse(string str)
        {
            BoundingBox result;
            try { result = Parse(str); }
            catch
            {
                return (false, new BoundingBox());
            }
            return (true, result);
        }
    }
}