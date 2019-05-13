using System.Windows;
using System.Collections;
using Terminal.Components;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.Specialized;

namespace Terminal.WPF.Controls
{
    /// <summary>
    /// Interaction logic for TerminalTabControl.xaml
    /// </summary>
    public partial class TerminalTabControl : UserControl
    {
        public TerminalTab Model
        {
            get => (TerminalTab)GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        }
        
        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register(nameof(Model), 
                typeof(TerminalTab), 
                typeof(TerminalTabControl), 
                new PropertyMetadata(null, ModelChanged));

        public TerminalTabControl()
        {
            InitializeComponent();
        }

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter || e.Key != Key.Return)
                return;
            var model = Model;
            if (model == null)
                return;
            model.Execute(input.Text);
            input.Text = null;
        }
        private void Lines_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    OnLinesAdded(e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    OnLinesCleared();
                    break;
            }
        }
        private void OnLinesCleared()
        {
            Dispatcher.Invoke(() => output.Clear());
        }
        private void OnLinesAdded(IList list)
        {
            Dispatcher.Invoke(() =>
            {
                foreach (var item in list)
                {
                    output.AppendText(item as string);
                    output.AppendText("\n");
                }
            });
        }

        private static void ModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as TerminalTabControl;
            var model = e.OldValue as TerminalTab;
            if (model != null)
                model.Lines.CollectionChanged -= control.Lines_CollectionChanged;
            model = e.NewValue as TerminalTab;
            if (model == null)
                return;
            control.DataContext = e.NewValue;
            model.Lines.CollectionChanged += control.Lines_CollectionChanged;
        }
    }
}