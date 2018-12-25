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
        public readonly GridLength Left;
        public readonly GridLength Top;
        public readonly GridLength Right;
        public readonly GridLength Bottom;

        public BoundingBox(GridLength all)
        {
            Left = all;
            Top = all;
            Right = all;
            Bottom = all;
        }
        public BoundingBox(GridLength horizontal, GridLength vertical)
        {
            Left = horizontal;
            Top = vertical;
            Right = horizontal;
            Bottom = vertical;
        }
        public BoundingBox(GridLength left, GridLength top, GridLength right, GridLength bottom)
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
            hashCode *= -152 + EqualityComparer<GridLength>.Default.GetHashCode(Left);
            hashCode *= -152 + EqualityComparer<GridLength>.Default.GetHashCode(Right);
            hashCode *= -152 + EqualityComparer<GridLength>.Default.GetHashCode(Top);
            hashCode *= -152 + EqualityComparer<GridLength>.Default.GetHashCode(Bottom);
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
                GridLength length = GridLength.Parse(splited[0]);
                return new BoundingBox(length);
            }
            else if (splited.Length == 2)
            {
                GridLength horizontal = GridLength.Parse(splited[0]);
                GridLength vertical = GridLength.Parse(splited[1]);
                return new BoundingBox(horizontal, vertical);
            }
            else if (splited.Length == 4)
            {
                GridLength left = GridLength.Parse(splited[0]);
                GridLength top = GridLength.Parse(splited[1]);
                GridLength right = GridLength.Parse(splited[2]);
                GridLength bottom = GridLength.Parse(splited[3]);
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