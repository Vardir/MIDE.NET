using System.Windows;
using MIDE.Application;
using MIDE.WPFApp.Windows;
using MIDE.WPFApp.RelayCommands;
using ICommand = System.Windows.Input.ICommand;

namespace MIDE.WPFApp
{
    public class WindowViewModel : BaseWindowViewModel
    {
        protected WindowDockPosition dockPosition = WindowDockPosition.Undocked;

        public bool Borderless => window.WindowState == WindowState.Maximized || dockPosition != WindowDockPosition.Undocked;
        public int ResizeBorder => Borderless ? 0 : 6;
        public override int OuterMarginSize
        {
            get => Borderless ? 0 : outerMarginSize;
            set => outerMarginSize = value;
        }
        public override GridLength TitleHeightGridLength => new GridLength(TitleHight + ResizeBorder);
        public Thickness ResizeBorderThickness => new Thickness(ResizeBorder + OuterMarginSize);
        public AppKernel Kernel => AppKernel.Instance;

        public ICommand MinimizeCommand { get; private set; }
        public ICommand MaximizeCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        public ICommand MenuCommand { get; private set; }

        public WindowViewModel(Window window) : base(window)
        {
            window.StateChanged += (sender, e) =>
            {
                WindowResized();
            };
            
            MinimizeCommand = new WindowsRelayCommand(() => window.WindowState = WindowState.Minimized);
            MaximizeCommand = new WindowsRelayCommand(() => window.WindowState ^= WindowState.Maximized);
            CloseCommand = new WindowsRelayCommand(AppKernel.Instance.Exit);
            MenuCommand = new WindowsRelayCommand(() => SystemCommands.ShowSystemMenu(window, GetMousePosition()));

            //Fix window resize issue
            var resizer = new WindowResizer(window);
            resizer.WindowDockChanged += (dock) =>
            {
                // Store last position
                dockPosition = dock;
                WindowResized();
            };
        }

        protected void WindowResized()
        {
            OnPropertyChanged(nameof(Borderless));
            OnPropertyChanged(nameof(ResizeBorderThickness));
            OnPropertyChanged(nameof(OuterMarginSize));
            OnPropertyChanged(nameof(OuterMarginSizeThickness));
        }
    }
}