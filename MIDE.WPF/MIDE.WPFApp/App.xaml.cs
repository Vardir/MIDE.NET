using System;
using MIDE.Helpers;
using System.Windows;
using MIDE.Application;
using MIDE.WPFApp.Helpers;
using MIDE.WPFApp.Services;
using MIDE.WPFApp.FileSystem;
using MIDE.WPFApp.Initializers;
using MIDE.Application.Attrubites;
using System.Windows.Media;

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
        public WpfUIManager UIManager { get; private set; }

        public void LoadTheme()
        {
            ResourceDictionary colors = UIManager.LoadTheme();
            ResourceDictionary brushes = Resources.MergedDictionaries.Find(rd => rd.Source.OriginalString.Contains("Brushes.xaml"));
            Resources.MergedDictionaries.Remove(brushes);
            brushes.MergedDictionaries[0].Update(colors);
            Resources.MergedDictionaries.Add(brushes);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Setup();
            Kernel.Start();
            LoadTheme();
            base.OnStartup(e);
            MainWindow window = new MainWindow();
            MainWindow = window;
            window.ViewModel.Title = "WPFTemplate";
            MainWindow.Show();
        }
        protected override void OnExit(ExitEventArgs e)
        {
            if (!kernelStopped)
                Kernel.Exit();
            MainWindow?.Close();
            base.OnExit(e);
        }
        
        private void Setup()
        {
            Kernel.ApplicationExit += Kernel_ApplicationExit;
            Kernel.FileManager = new WinFileManager();
            Kernel.SystemClipboard = WindowsClipboard.Instance;
            UIManager = new WpfUIManager();
            Kernel.UIManager = UIManager;
            
            Kernel.Initializers.Add(new ApplicationMenuInitializer(Kernel));
            Kernel.Initializers.Add(new TabSectionInitializer());

            Kernel.FileManager.AddOrUpdate(MIDE.FileSystem.ApplicationPath.Logs, "root\\logs");
        }        

        private void Kernel_ApplicationExit()
        {
            MainWindow.Close();
            kernelStopped = true;
        }        
    }
}