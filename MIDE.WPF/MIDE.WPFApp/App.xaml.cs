using System.Windows;
using MIDE.Application;
using MIDE.WPFApp.Services;
using MIDE.WPFApp.FileSystem;
using MIDE.Application.Attrubites;

[assembly: ApplicationProperties("wpftemplate")]

namespace MIDE.WPFApp
{
    /// <summary>
    /// Interaction logic for WPFApplication.xaml
    /// </summary>
    public partial class WPFApplication : System.Windows.Application
    {
        private bool kernelStopped;

        public AppKernel Kernel => AppKernel.Instance;

        protected override void OnStartup(StartupEventArgs e)
        {
            Kernel.FileManager = new WinFileManager();
            Kernel.SystemBuffer = WindowsBuffer.Instance;
            Kernel.Start();
            Kernel.ApplicationExit += Kernel_ApplicationExit;
            var window = new MainWindow();
            MainWindow = window;
            window.ViewModel.Title = "WPFTemplate";
            MainWindow.Show();
            base.OnStartup(e);
        }

        private void Kernel_ApplicationExit()
        {
            MainWindow.Close();
            kernelStopped = true;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (!kernelStopped)
                Kernel.Exit();
            MainWindow?.Close();
            base.OnExit(e);
        }
    }
}