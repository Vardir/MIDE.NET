using System;
using System.Windows;
using MIDE.CustomImpl;
using System.Windows.Media;
using NodeGraphs.DataModels;
using NodeGraphs.Components;
using System.Windows.Controls;
using System.Collections.Specialized;
using System.Windows.Controls.Primitives;

namespace NodeGraphs.WPF.Controls
{
    public class GraphCanvasControl : ItemsControl, IBounds
    {
        private Pen xAxisPen;
        private Pen yAxisPen;
        private Queue<GraphComponent> lastAddedGraphComponents;

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
            lastAddedGraphComponents = new Queue<GraphComponent>();
            ItemsPanel = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(Grid)));

            ((INotifyCollectionChanged)Items).CollectionChanged += GraphCanvasControl_CollectionChanged;
            ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;

            xAxisPen = new Pen(XAxisBrush, 1);
            yAxisPen = new Pen(YAxisBrush, 1);

        }

        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                return;
            lastAddedGraphComponents.ForEach((component) =>
            {
                var contentPresenter = ItemContainerGenerator.ContainerFromItem(component) as ContentPresenter;
                if (contentPresenter == null)
                    return;
                contentPresenter.Loaded += ContentPresenter_Loaded;
            });
        }

        private void ContentPresenter_Loaded(object sender, RoutedEventArgs e)
        {
            var contentPresenter = sender as ContentPresenter;
            contentPresenter.Loaded -= ContentPresenter_Loaded;
            int count = VisualTreeHelper.GetChildrenCount(contentPresenter);
            if (count == 0)
                return;
            var child = VisualTreeHelper.GetChild(contentPresenter, 0);
            if (child is GraphNodeControl nodeControl)
            {
                ResetChild(nodeControl);
            }
        }

        public void UpdateLocation(GraphNodeControl control)
        {
            var overlapingSide = control.GetOverlapingSide(this, out int points);
            EnsureBorders(overlapingSide, points + 5);

            InternalUpdateLocation(control);
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
            var model = element.Model;
            translate.X = XAxisBase + model.Location.x - model.Pivot.x;
            translate.Y = YAxisBase - model.Location.y - model.Pivot.y;
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
            if (value is GraphComponent component)
                lastAddedGraphComponents.Enqueue(component);
        }
        private void OnChildRemove(object value)
        {
            return;//TODO find exact control and clear it's properties as below
            //element.Container = null;
        }
        private void OnReset()
        {
            return;
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
            model = e.NewValue as GraphCanvas;
            if (model == null)
                return;
            control.ItemsSource = model.Components;
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