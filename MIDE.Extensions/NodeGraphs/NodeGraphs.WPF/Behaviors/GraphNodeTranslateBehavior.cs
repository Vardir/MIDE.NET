using System.Windows;
using System.Windows.Input;
using NodeGraphs.WPF.Controls;

namespace NodeGraphs.WPF.Behaviors
{
    public class GraphNodeTranslateBehavior : Behavior<GraphNodeControl>
    {
        private bool isDragging = false;
        private System.Windows.Point mouseOffset;
        private UIElement elementParent;

        protected override void OnAttached()
        {
            elementParent = AssociatedObject.Parent as UIElement;

            AssociatedObject.MouseMove += AssociatedObject_MouseMove;
            AssociatedObject.MouseLeave += AssociatedObject_MouseLeave;
            AssociatedObject.MouseLeftButtonUp += AssociatedObject_MouseLeftButtonUp;
            AssociatedObject.MouseLeftButtonDown += AssociatedObject_MouseLeftButtonDown;
        }
        protected override void OnDetached()
        {
            mouseOffset = new System.Windows.Point();
            isDragging = false;
            elementParent = null;

            AssociatedObject.MouseMove -= AssociatedObject_MouseMove;
            AssociatedObject.MouseLeave -= AssociatedObject_MouseLeave;
            AssociatedObject.MouseLeftButtonUp -= AssociatedObject_MouseLeftButtonUp;
            AssociatedObject.MouseLeftButtonDown -= AssociatedObject_MouseLeftButtonDown;
        }

        private void AssociatedObject_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDragging)
                return;

            System.Windows.Point point = e.GetPosition(elementParent);

            int dx = (int)(point.X - mouseOffset.X);
            int dy = (int)(point.Y - mouseOffset.Y);

            mouseOffset = point;
            AssociatedObject.Move(dx, dy);
        }
        private void AssociatedObject_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!isDragging)
                return;

            AssociatedObject.ReleaseMouseCapture();
            isDragging = false;
        }
        private void AssociatedObject_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!isDragging)
                return;

            AssociatedObject.ReleaseMouseCapture();
            isDragging = false;
        }
        private void AssociatedObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;

            mouseOffset = e.GetPosition(elementParent);
            AssociatedObject.CaptureMouse();            
        }
    }
}