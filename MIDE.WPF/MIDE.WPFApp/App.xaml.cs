using System.Windows;
using MIDE.WPFApp.FileSystem;
using MIDE.Standard.Application;
using MIDE.Standard.Application.Attrubites;

[assembly: ApplicationProperties("wpftemplate")]

namespace MIDE.WPFApp
{
    /// <summary>
    /// Interaction logic for WPFApplication.xaml
    /// </summary>
    public partial class WPFApplication : Application
    {
        public AppKernel Kernel => AppKernel.Instance;

        protected override void OnStartup(StartupEventArgs e)
        {
            Kernel.FileManager = new WinFileManager();
            Kernel.Start();
            var window = new MainWindow();
            MainWindow = window;
            window.ViewModel.Title = "WPFTemplate";
            MainWindow.Show();
            base.OnStartup(e);
        }
        protected override void OnExit(ExitEventArgs e)
        {
            Kernel.Exit();
            base.OnExit(e);
        }
    }
}