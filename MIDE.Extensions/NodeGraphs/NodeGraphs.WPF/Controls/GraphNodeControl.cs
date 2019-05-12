using System;
using System.Windows;
using System.Windows.Media;
using NodeGraphs.Components;
using NodeGraphs.DataModels;
using System.ComponentModel;
using System.Windows.Controls;
using NodeGraphs.WPF.Behaviors;

namespace NodeGraphs.WPF.Controls
{
    public abstract class GraphNodeControl : ContentControl
    {
        public GraphNode Model
        {
            get => (GraphNode)GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        }
        public GraphCanvasControl Container
        {
            get => (GraphCanvasControl)GetValue(ContainerProperty);
            set => SetValue(ContainerProperty, value);
        }
        public TranslateTransform TranslateTransform { get; }
        
        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register(nameof(Model), 
                typeof(GraphNode), typeof(GraphNodeControl), 
                new PropertyMetadata(null, ModelChanged));

        public static readonly DependencyProperty ContainerProperty =
            DependencyProperty.Register(nameof(Container),
                typeof(GraphCanvasControl), typeof(GraphNodeControl),
                new PropertyMetadata(null, ContainerChanged));

        public GraphNodeControl()
        {            
            TranslateTransform = new TranslateTransform();
            RenderTransform = TranslateTransform;
            Behavior.Attach(new GraphNodeTranslateBehavior(), this);
        }

        public void Move(int dx, int dy)
        {
            var model = Model;
            if (model == null)
                return;
            model.Location = model.Location.Translate(dx, -dy);
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
        protected void Model_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(GraphNode.Location))
                OnLocationChanged();
            else if (args.PropertyName == nameof(GraphNode.Pivot))
                OnPivotChanged();
        }

        private static void ModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GraphNodeControl control = d as GraphNodeControl;
            GraphNode model = e.OldValue as GraphNode;
            if (model != null)
            {
                model.PropertyChanged -= control.Model_PropertyChanged;
            }
            model = e.NewValue as GraphNode;
            if (model == null)
                return;
            model.PropertyChanged += control.Model_PropertyChanged;
        }
        private static void ContainerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}