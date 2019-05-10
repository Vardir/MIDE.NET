using System;
using System.Windows;
using System.Windows.Media;
using NodeGraphs.DataModels;
using NodeGraphs.Components;
using System.Windows.Controls;
using System.Collections.Specialized;

namespace NodeGraphs.WPF.Controls
{
    public class GraphCanvasControl : ItemsControl, IBounds
    {
        private Pen xAxisPen;
        private Pen yAxisPen;

        private int XAxisBase => Model.XAxis.Basis - Model.XAxis.Start;
        private int YAxisBase => Model.YAxis.End - Model.YAxis.Basis;

        public int LeftBound => Model.XAxis.Start;
        public int RightBound => Model.XAxis.End;
        public int TopBound => Model.YAxis.End;
        public int BottomBound => Model.YAxis.Start;

        public Brush GridBackground
        {
            get => (Brush)GetValue(GridBackgroundProperty);
            set => SetValue(GridBackgroundProperty, value);
        }
        public Brush XAxisBrush
        {
            get => (Brush)GetValue(XAxisBrushProperty);
            set => SetValue(XAxisBrushProperty, value);
        }
        public Brush YAxisBrush
        {
            get => (Brush)GetValue(YAxisBrushProperty);
            set => SetValue(YAxisBrushProperty, value);
        }
        public GraphCanvas Model
        {
            get => (GraphCanvas)GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        }

        #region Dependency Properties
        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register(nameof(Model), typeof(GraphCanvas), 
                typeof(GraphCanvasControl), new PropertyMetadata(null, ModelChanged));

        public static readonly DependencyProperty GridBackgroundProperty =
            DependencyProperty.Register(nameof(GridBackgroundProperty),
                typeof(Brush), typeof(GraphCanvasControl),
                new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty XAxisBrushProperty =
            DependencyProperty.Register(nameof(XAxisBrush),
                typeof(Brush), typeof(GraphCanvasControl),
                new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender, XAxisBrushChanged));

        public static readonly DependencyProperty YAxisBrushProperty =
            DependencyProperty.Register(nameof(YAxisBrush),
                typeof(Brush), typeof(GraphCanvasControl),
                new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender, YAxisBrushChanged));
        #endregion

        public GraphCanvasControl()
        {
            Model = new GraphCanvas("canvas");
            ItemsPanel = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(Grid)));

            ((INotifyCollectionChanged)Items).CollectionChanged += GraphCanvasControl_CollectionChanged;

            xAxisPen = new Pen(XAxisBrush, 1);
            yAxisPen = new Pen(YAxisBrush, 1);
        }

        public void UpdateLocation(GraphNodeControl element)
        {
            var overlapingSide = element.GetOverlapingSide(this, out int points);
            EnsureBorders(overlapingSide, points + 5);

            InternalUpdateLocation(element);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(GridBackground, null, new Rect(0, 0, Model.XAxis.Length, Model.YAxis.Length));
            drawingContext.DrawLine(xAxisPen,
                new System.Windows.Point(0, YAxisBase), new System.Windows.Point(Model.XAxis.Length, YAxisBase));
            drawingContext.DrawLine(yAxisPen,
                new System.Windows.Point(XAxisBase, 0), new System.Windows.Point(XAxisBase, Model.YAxis.Length));
        }

        private void UpdateSize()
        {
            Width = Model.XAxis.Length;
            Height = Model.YAxis.Length;
            UpdateLocations();
        }
        private void InternalUpdateLocation(GraphNodeControl element)
        {
            var translate = element.TranslateTransform;
            translate.X = XAxisBase + element.Location.x - element.Pivot.x;
            translate.Y = YAxisBase - element.Location.x - element.Pivot.x;
        }
        private void UpdateLocations()
        {
            int len = Items.Count;
            for (int i = 0; i < len; i++)
            {
                if (Items[i] is GraphNodeControl element)
                    InternalUpdateLocation(element);
            }
        }
        private void ResetChild(GraphNodeControl element)
        {
            element.Container = this;
            element.VerticalAlignment = VerticalAlignment.Top;
            element.HorizontalAlignment = HorizontalAlignment.Left;
            UpdateLocation(element);
        }
        private void EnsureBorders(GridSide gridSide, int points)
        {
            if (gridSide == GridSide.None)
                return;

            if (gridSide.HasFlag(GridSide.Top))
                Model.YAxis.End += points;
            if (gridSide.HasFlag(GridSide.Bottom))
                Model.YAxis.Start -= points;

            if (gridSide.HasFlag(GridSide.Left))
                Model.XAxis.Start -= points;
            if (gridSide.HasFlag(GridSide.Right))
                Model.XAxis.End += points;
        }

        private void OnChildAdd(object value)
        {
            if (value == null || !(value is GraphNodeControl element))
                throw new ArgumentException();

            ResetChild(element);
        }
        private void OnChildRemove(object value)
        {
            if (value == null || !(value is GraphNodeControl element))
                throw new ArgumentException();

            element.Container = null;
        }
        private void OnReset()
        {
            foreach (var item in Items)
            {
                if (item is GraphNodeControl element)
                {
                    ResetChild(element);
                }
            }
        }

        private void GraphCanvasControl_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    OnChildAdd(e.NewItems[0]); break;
                case NotifyCollectionChangedAction.Remove:
                    OnChildRemove(e.NewItems[0]); break;
                case NotifyCollectionChangedAction.Reset:
                    OnReset(); break;
            }
        }
        private void YAxis_LengthChanged(Axis axis) => UpdateSize();        
        private void XAxis_LengthChanged(Axis axis) => UpdateSize();

        private static void ModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GraphCanvasControl control = d as GraphCanvasControl;
            GraphCanvas model = e.OldValue as GraphCanvas;
            if (model != null)
            {
                model.XAxis.LengthChanged -= control.XAxis_LengthChanged;
                model.YAxis.LengthChanged -= control.YAxis_LengthChanged;
            }
            if (model == null)
                return;
            control.ItemsSource = model.Components;
            model = e.NewValue as GraphCanvas;
            model.XAxis.LengthChanged += control.XAxis_LengthChanged;
            model.YAxis.LengthChanged += control.YAxis_LengthChanged;
            control.UpdateSize();
        }
        private static void XAxisBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as GraphCanvasControl).xAxisPen.Brush = (Brush)e.NewValue;
        }
        private static void YAxisBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as GraphCanvasControl).yAxisPen.Brush = (Brush)e.NewValue;
        }
    }
}
