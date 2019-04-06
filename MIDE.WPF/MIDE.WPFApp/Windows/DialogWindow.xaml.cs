using System;
using System.Windows;
using MIDE.WPFApp.Helpers;
using MIDE.API.Components;
using System.Windows.Controls;

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
        
        public static (DialogResult result, T value) Show<T>(BaseDialogBox<T> dialogBox)
        {
            DialogWindow window = new DialogWindow();
            window.ViewModel.Title = dialogBox.Title;
            window.box.Content = dialogBox;
            window.box.DataContext = dialogBox;
            //window.grid.DataContext = dialogBox;
            //window.body.Content = dialogBox;
            //window.buttons.Content = dialogBox.DialogButtons;
            //window.validationErrors.ItemsSource = dialogBox.ValidationErrors;
            dialogBox.ResultSelected += window.Close;
            bool? _ = window.ShowDialog();
            dialogBox.ResultSelected -= window.Close;
            return (dialogBox.SelectedResult, dialogBox.GetData());
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
