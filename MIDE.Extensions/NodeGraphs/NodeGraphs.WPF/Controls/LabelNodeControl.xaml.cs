using NodeGraphs.DataModels;
using NodeGraphs.WPF.Helpers;

namespace NodeGraphs.WPF.Controls
{
    /// <summary>
    /// Interaction logic for LabelNodeControl.xaml
    /// </summary>
    public partial class LabelNodeControl : GraphNodeControl, IRect
    {
        #region Rect Properties
        public int LeftEdge => (int)(Location.x - Pivot.x);
        public int RightEdge => (int)(Location.x + GetWidth() - Pivot.x);
        public int TopEdge => (int)(Location.y + Pivot.y);
        public int BottomEdge => (int)(Location.y - GetHeight() + Pivot.y);
        #endregion

        public LabelNodeControl()
        {
            InitializeComponent();
        }
        
        public int GetWidth() => double.IsNaN(Width) ? (int)ActualWidth : (int)Width;
        public int GetHeight() => double.IsNaN(Height) ? (int)ActualHeight : (int)Height;

        public override GridSide GetOverlapingSide(IBounds bounds, out int points)
        {
            return GraphNodeHelpers.GetOverlapingSide(this, bounds, out points);
        }
    }
}
