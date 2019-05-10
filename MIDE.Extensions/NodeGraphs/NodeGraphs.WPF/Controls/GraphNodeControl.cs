using System.Windows;
using System.Windows.Media;
using NodeGraphs.DataModels;
using System.Windows.Controls;
using NodeGraphs.WPF.Behaviors;
using NodeGraphs.WPF.Helpers;

namespace NodeGraphs.WPF.Controls
{
    public abstract class GraphNodeControl : ContentControl
    {
        private Point pivot;
        private Point location;

        public bool IsFixedLocation { get; set; }
        public Point Pivot
        {
            get => pivot;
            set
            {
                pivot = value;
                OnPivotChanged();
            }
        }
        public Point Location
        {
            get => location;
            set
            {
                location = value;
                OnLocationChanged();
            }
        }
        public GraphCanvasControl Container
        {
            get => (GraphCanvasControl)GetValue(ContainerProperty);
            set => SetValue(ContainerProperty, value);
        }
        public TranslateTransform TranslateTransform { get; }

        public static readonly DependencyProperty ContainerProperty =
            DependencyProperty.Register(nameof(Container),
                typeof(GraphCanvasControl), typeof(GraphNodeControl),
                new PropertyMetadata(null));

        public GraphNodeControl()
        {
            TranslateTransform = new TranslateTransform();
            RenderTransform = TranslateTransform;
            Behavior.Attach(new GraphNodeTranslateBehavior(), this);
        }

        public void Move(int dx, int dy)
        {
            if (IsFixedLocation)
                return;

            location = location.Translate(dx, -dy);

            OnLocationChanged();
        }

        public abstract GridSide GetOverlapingSide(IBounds bounds, out int points);

        protected void OnPivotChanged()
        {
            if (Container == null) return;

            Container.UpdateLocation(this);
        }
        protected void OnLocationChanged()
        {
            if (Container == null) return;

            Container.UpdateLocation(this);
        }
    }
}