using System.Windows;
using System.Windows.Media;

namespace MIDE.WPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public WindowViewModel ViewModel { get; }

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new WindowViewModel(this);
            DataContext = ViewModel;
        }

        private void Window_Deactivated(object sender, System.EventArgs e)
        {
            ViewModel.DimmableOverlayVisible = true;
        }
        private void Window_Activated(object sender, System.EventArgs e)
        {
            ViewModel.DimmableOverlayVisible = false;
        }
    }
}
