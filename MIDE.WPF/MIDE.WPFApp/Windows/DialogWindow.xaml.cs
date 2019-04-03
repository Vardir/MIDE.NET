using System;
using System.Windows;
using MIDE.API.Components;

namespace MIDE.WPFApp
{
    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window
    {
        public DialogWindowViewModel ViewModel { get; }

        public DialogWindow()
        {
            InitializeComponent();
            ViewModel = new DialogWindowViewModel(this);
            DataContext = ViewModel;
        }
        
        public static (DialogResult result, T value) Show<T>(BaseDialogBox<T> dialogWindow)
        {
            DialogWindow window = new DialogWindow();
            window.ViewModel.Title = dialogWindow.Title;
            window.body.Content = dialogWindow;
            window.buttons.ItemsSource = dialogWindow.DialogButtons;
            window.validationErrors.ItemsSource = dialogWindow.ValidationErrors;
            dialogWindow.ResultSelected += window.Close;
            bool? _ = window.ShowDialog();
            dialogWindow.ResultSelected -= window.Close;
            return (dialogWindow.SelectedResult, dialogWindow.GetData());
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            ViewModel.DimmableOverlayVisible = true;
        }
        private void Window_Activated(object sender, EventArgs e)
        {
            ViewModel.DimmableOverlayVisible = false;
        }
    }
}
