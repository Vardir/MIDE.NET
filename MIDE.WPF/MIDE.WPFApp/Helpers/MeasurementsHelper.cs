using System;
using WIN = System.Windows;
using APIM = MIDE.API.Measurements;

namespace MIDE.WPFApp.Helpers
{
    public static class MeasurementsHelper
    {
        public static WIN.GridLength ToWindows(this APIM.GridLength gridLength)
        {
            switch (gridLength.Type)
            {
                case APIM.GridLengthType.Auto:
                    return new WIN.GridLength(0, WIN.GridUnitType.Auto);
                default:
                    return new WIN.GridLength(gridLength.Value, gridLength.Type.ToWindows());
            }
        }
        public static WIN.Thickness ToWindows(this APIM.BoundingBox boundingBox)
        {
            return new WIN.Thickness()
            {
                Bottom = boundingBox.Bottom,
                Top = boundingBox.Top,
                Right = boundingBox.Right,
                Left = boundingBox.Left
            };
        }
        public static WIN.GridUnitType ToWindows(this APIM.GridLengthType gridLengthType)
        {
            switch (gridLengthType)
            {
                case APIM.GridLengthType.Auto: return WIN.GridUnitType.Auto;
                case APIM.GridLengthType.Relative: return WIN.GridUnitType.Star;
                case APIM.GridLengthType.Points: return WIN.GridUnitType.Pixel;
                case APIM.GridLengthType.None: return WIN.GridUnitType.Auto;
                default:
                    throw new ArgumentException("Unsupported measurement");
            }
        }
    }
}