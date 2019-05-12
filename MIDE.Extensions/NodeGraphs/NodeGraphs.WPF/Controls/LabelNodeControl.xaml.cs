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
        public int LeftEdge => (Model.Location.x - Model.Pivot.x);
        public int RightEdge => (Model.Location.x + GetWidth() - Model.Pivot.x);
        public int TopEdge => (Model.Location.y + Model.Pivot.y);
        public int BottomEdge => (Model.Location.y - GetHeight() + Model.Pivot.y);
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
