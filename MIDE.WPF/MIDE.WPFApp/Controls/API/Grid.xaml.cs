using System.Windows;
using MIDE.API.Components;
using MIDE.WPFApp.Helpers;
using System.ComponentModel;
using System.Windows.Controls;

namespace MIDE.WPFApp.Controls
{
    /// <summary>
    /// Interaction logic for Grid.xaml
    /// </summary>
    public partial class Grid : UserControl
    {
        private System.Windows.Controls.Grid container;

        public GridLayout Layout
        {
            get => (GridLayout)GetValue(LayoutProperty);
            set => SetValue(LayoutProperty, value);
        }
        
        public static readonly DependencyProperty LayoutProperty =
            DependencyProperty.Register("Layout", typeof(GridLayout), typeof(Grid), 
                new PropertyMetadata(null, LayoutPropertyChanged));

        public Grid()
        {
            InitializeComponent();
            container = wrapper.ItemsPanel.LoadContent() as System.Windows.Controls.Grid;
        }

        private void ClearContents()
        {
            container.RowDefinitions.Clear();
            container.ColumnDefinitions.Clear();
        }
        private void UpdateToLayout()
        {
            var layout = Layout;
            if (layout == null)
                return;
            var margin = layout.RowMargin.ToWindows();
            for (int i = 0; i < layout.Rows.Count; i++)
            {
                var row = layout.Rows[i];
                var rowDef = new RowDefinition()
                {
                    Height = row.Height.ToWindows(),
                    //MinHeight = row.MinHeight.Value,
                    //MaxHeight = row.MaxHeight.Value
                };
                container.RowDefinitions.Add(rowDef);
                if (i < layout.Rows.Count - 1)
                {
                    container.RowDefinitions.Add(new RowDefinition()
                    {
                        Height = margin
                    });
                }
            }
            margin = layout.ColumnMargin.ToWindows();
            for (int i = 0; i < layout.Columns.Count; i++)
            {
                var column = layout.Columns[i];
                var colDef = new ColumnDefinition()
                {
                    Width = column.Width.ToWindows(),
                    //MinWidth = column.MinWidth.Value,
                    //MaxWidth = column.MaxWidth.Value
                };
                container.ColumnDefinitions.Add(colDef);
                if (i < layout.Columns.Count - 1)
                {
                    container.ColumnDefinitions.Add(new ColumnDefinition()
                    {
                        Width = margin
                    });
                }
            }
        }
        private void UpdateRowMargin()
        {
            var layout = Layout;
            if (layout == null)
                return;

            var margin = layout.RowMargin.ToWindows();
            for (int i = 1; i < layout.Rows.Count; i+=2)
            {
                container.RowDefinitions[i].Height = margin;
            }
        }
        private void UpdateColumnMargin()
        {
            var layout = Layout;
            if (layout == null)
                return;

            var margin = layout.ColumnMargin.ToWindows();
            for (int i = 1; i < layout.Columns.Count; i += 2)
            {
                container.ColumnDefinitions[i].Width = margin;
            }
        }

        private void Layout_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "RowMargin":
                    UpdateRowMargin(); break;
                case "ColumnMargin":
                    UpdateColumnMargin(); break;
            }
        }

        private static void LayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as Grid;
            grid.ClearContents();
            grid.UpdateToLayout();
            var layout = e.NewValue as GridLayout;
            if (layout != null)
            {
                layout.PropertyChanged += grid.Layout_PropertyChanged;
            }
            grid.DataContext = layout;
            layout = e.OldValue as GridLayout;
            if (layout != null)
            {
                layout.PropertyChanged -= grid.Layout_PropertyChanged;
            }
        }
    }
}