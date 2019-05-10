using System;
using NodeGraphs.DataModels;

namespace NodeGraphs.WPF.Helpers
{
    public static class GraphNodeHelpers
    {
        public static GridSide GetOverlapingSide(IRect rect, IBounds bounds, out int points)
        {
            GridSide side = GridSide.None;
            points = 0;

            int offset = rect.LeftEdge - bounds.LeftBound;
            if (offset < 0)
            {
                side = GridSide.Left;
                points = -offset;
            }
            else
            {
                offset = bounds.RightBound - rect.RightEdge;
                if (offset < 0)
                {
                    side = GridSide.Right;
                    points = -offset;
                }
            }

            offset = bounds.TopBound - rect.TopEdge;
            if (offset < 0)
            {
                side |= GridSide.Top;
                points = Math.Max(points, -offset);
            }
            else
            {
                offset = rect.BottomEdge - bounds.BottomBound;
                if (offset < 0)
                {
                    side |= GridSide.Bottom;
                    points = Math.Max(points, -offset);
                }
            }
            return side;
        }
    }
}