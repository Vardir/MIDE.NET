using System;
using System.Windows;
using MIDE.WPFApp.Windows;
using System.Windows.Input;
using MIDE.WPFApp.RelayCommands;
using MIDE.Standard.API.ViewModels;
using System.Runtime.InteropServices;
using ICommand = System.Windows.Input.ICommand;
using MIDE.Standard.Application;

namespace MIDE.WPFApp
{
    public class WindowViewModel : BaseViewModel
    {
        private int outerMarginSize = 10;
        private bool dimmableOverlayVisible;
        private WindowDockPosition dockPosition = WindowDockPosition.Undocked;
        private string title;
        private Window window;

        public bool Borderless => window.WindowState == WindowState.Maximized || dockPosition != WindowDockPosition.Undocked;
        public bool DimmableOverlayVisible
        {
            get => dimmableOverlayVisible;
            set
            {
                if (dimmableOverlayVisible != value)
                {
                    dimmableOverlayVisible = value;
                    OnPropertyChanged(nameof(DimmableOverlayVisible));
                }
            }
        }
        public int ResizeBorder => Borderless ? 0 : 6;
        public int OuterMarginSize
        {
            get => Borderless ? 0 : outerMarginSize;
            set => outerMarginSize = value;
        }
        public int TitleHight { get; set; } = 30;
        public double WindowMinimumWidth { get; set; } = 800;
        public double WindowMinimumHeight { get; set; } = 500;
        public string Title
        {
            get => title;
            set
            {
                if (value == title)
                    return;
                title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
        public GridLength TitleHeightGridLength => new GridLength(TitleHight + ResizeBorder);
        public Thickness ResizeBorderThickness => new Thickness(ResizeBorder + OuterMarginSize);
        public Thickness OuterMarginSizeThickness => new Thickness(OuterMarginSize);
        public Thickness InnerContentPadding { get; set; } = new Thickness(0);
        public AppKernel Kernel => AppKernel.Instance;

        public ICommand MinimizeCommand { get; private set; }
        public ICommand MaximizeCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        public ICommand MenuCommand { get; private set; }

        public WindowViewModel(Window window)
        {
            this.window = window;

            window.StateChanged += (sender, e) =>
            {
                WindowResized();
            };

            MinimizeCommand = new WindowsRelayCommand(() => window.WindowState = WindowState.Minimized);
            MaximizeCommand = new WindowsRelayCommand(() => window.WindowState ^= WindowState.Maximized); //if window is already maximized, makes it normal
            CloseCommand = new WindowsRelayCommand(Kernel.Exit);
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

        private void WindowResized()
        {
            OnPropertyChanged(nameof(Borderless));
            OnPropertyChanged(nameof(ResizeBorderThickness));
            OnPropertyChanged(nameof(OuterMarginSize));
            OnPropertyChanged(nameof(OuterMarginSizeThickness));
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
        private static Point GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }
        private Point GetMousePosition2()
        {
            var position = Mouse.GetPosition(window);
            return new Point(position.X + window.Left, position.Y + window.Top);
        }
    }
}