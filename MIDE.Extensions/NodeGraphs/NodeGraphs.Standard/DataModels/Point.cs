using System;

namespace NodeGraphs
{
    public struct Point : IEquatable<Point>
    {
        public readonly int x;
        public readonly int y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Point Translate(int dx, int dy)
        {
            return new Point(x + dx, y + dy);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Point point))
                return false;
            
            return x == point.x &&
                   y == point.y;
        }
        public bool Equals(Point other)
        {
            return x == other.x &&
                   y == other.y;
        }
        public override int GetHashCode()
        {
            var hashCode = 150293;
            hashCode = hashCode * -152425 + x;
            hashCode = hashCode * -134295 + y;
            return hashCode;
        }

        public static bool operator==(Point p1, Point p2)
        {
            return p1.x == p2.x && p1.y == p2.y;
        }
        public static bool operator!=(Point p1, Point p2)
        {
            return p1.x != p2.x || p1.y != p2.y;
        }
    }
}