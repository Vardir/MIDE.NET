using System;
using System.Windows;
using SWC = System.Windows.Controls;

namespace MIDE.WPFApp.Controls
{
    public class ContentPresenter : SWC.ContentPresenter
    {
        public ContentPresenter()
        {
            DataContextChanged += ContentPresenter_DataContextChanged;
        }

        private void ContentPresenter_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateLayout();
        }
    }
}