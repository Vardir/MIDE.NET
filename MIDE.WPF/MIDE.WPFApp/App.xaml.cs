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
        
        protected override void OnStartup(StartupEventArgs e)
        {
            Setup();
            Kernel.Start();
            Resources = GetResources();
            base.OnStartup(e);
            MainWindow window = new MainWindow();
            MainWindow = window;
            window.ViewModel.Title = "WPFTemplate";
            MainWindow.Show();
            
            ResourceDictionary colors = UIManager.LoadTheme();
            ResourceDictionary brushes = Resources.MergedDictionaries.Find(rd => rd.Source.AbsolutePath == "/Styles/Brushes.xaml");
            Resources.MergedDictionaries.Remove(brushes);
            brushes.MergedDictionaries[0].Update(colors);
            Resources.MergedDictionaries.Add(brushes);
            brushes["Window_HeaderBackground_Brush"] = Brushes.Red;
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
        }

        private void Kernel_ApplicationExit()
        {
            MainWindow.Close();
            kernelStopped = true;
        }

        private ResourceDictionary GetResources()
        {
            ResourceDictionary generalResource = new ResourceDictionary();

            ResourceDictionary brushes = new ResourceDictionary();
            brushes.Source = new Uri("pack://application:,,,/Styles/Brushes.xaml", UriKind.RelativeOrAbsolute);
            generalResource.MergedDictionaries.Add(brushes);

            ResourceDictionary fonts = new ResourceDictionary();
            fonts.Source = new Uri("pack://application:,,,/Styles/Fonts.xaml", UriKind.RelativeOrAbsolute);
            generalResource.MergedDictionaries.Add(fonts);

            ResourceDictionary shared = new ResourceDictionary();
            shared.Source = new Uri("pack://application:,,,/Styles/Shared.xaml", UriKind.RelativeOrAbsolute);
            generalResource.MergedDictionaries.Add(shared);

            ResourceDictionary controls = new ResourceDictionary();
            controls.Source = new Uri("pack://application:,,,/Styles/Controls.xaml", UriKind.RelativeOrAbsolute);
            generalResource.MergedDictionaries.Add(controls);

            ResourceDictionary defaultControls = new ResourceDictionary();
            defaultControls.Source = new Uri("pack://application:,,,/Styles/DefaultControls.xaml", UriKind.RelativeOrAbsolute);
            generalResource.MergedDictionaries.Add(defaultControls);

            ResourceDictionary windows = new ResourceDictionary();
            windows.Source = new Uri("pack://application:,,,/Styles/Windows.xaml", UriKind.RelativeOrAbsolute);
            generalResource.MergedDictionaries.Add(windows);

            return generalResource;
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