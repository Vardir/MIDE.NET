﻿using MIDE.Helpers;
using System.Windows;
using MIDE.Application;
using MIDE.WPF.Helpers;
using MIDE.WPF.Services;
using MIDE.WPF.FileSystem;
using MIDE.WPF.Initializers;
using MIDE.Application.Attributes;
using MIDE.FileSystem;

[assembly: ApplicationProperties("wpftemplate")]

namespace MIDE.WPF
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
        public void LoadUIExtensions()
        {
            foreach (var extension in UIManager.GetLoadedUIExtensions())
            {
                var dictionaries = extension.GetResourceDictionaries();
                foreach (var dictionary in dictionaries)
                {
                    Resources.MergedDictionaries.Add(dictionary);
                }
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Setup();
            Kernel.Start();
            FileSystemInfo.Instance.FilePropertiesExtractor = new WinFilePropertiesExtractor();
            LoadTheme();
            LoadUIExtensions();
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
    }
}