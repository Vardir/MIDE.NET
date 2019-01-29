using System.Windows;
using MIDE.Application;
using MIDE.WPFApp.Services;
using MIDE.WPFApp.FileSystem;
using MIDE.WPFApp.Initializers;
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
            Initialize();
            Kernel.Start();
            var window = new MainWindow();
            MainWindow = window;
            window.ViewModel.Title = "WPFTemplate";
            MainWindow.Show();
            base.OnStartup(e);
        }

        private void Initialize()
        {
            Kernel.ApplicationExit += Kernel_ApplicationExit;
            Kernel.FileManager = new WinFileManager();
            Kernel.SystemClipboard = WindowsClipboard.Instance;
            Kernel.UIManager = new WpfUIManager();

            Kernel.Initializers.Add(new ApplicationMenuInitializer(Kernel));
            Kernel.Initializers.Add(new TabSectionInitializer());
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