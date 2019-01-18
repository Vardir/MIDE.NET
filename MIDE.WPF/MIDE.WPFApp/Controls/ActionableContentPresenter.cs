using System.Windows;
using System.Windows.Controls;

namespace MIDE.WPFApp.Controls
{
    public class ActionableContentPresenter : System.Windows.Controls.Grid
    {
        protected ContentPresenter ChildPresenter
        {
            get => Children.Count == 1 ? Children[0] as ContentPresenter : null;
            set
            {
                Children.Clear();
                Children.Add(value);
            }
        }

        public ActionableContentPresenter()
        {
            DataContextChanged += ActionableContentPresenter_DataContextChanged;
        }

        private void ActionableContentPresenter_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ChildPresenter = new ContentPresenter()
            {
                Content = e.NewValue
            };
        }
    }
}