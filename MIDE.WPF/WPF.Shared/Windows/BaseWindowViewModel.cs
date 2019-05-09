using System;
using System.Windows;
using MIDE.API.ViewModels;
using System.Windows.Input;
using System.Runtime.InteropServices;

namespace MIDE.WPF.Windows
{
    public class BaseWindowViewModel : BaseViewModel
    {
        protected int outerMarginSize = 10;
        protected bool dimmableOverlayVisible;
        protected string title;
        protected Window window;

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
        public virtual int OuterMarginSize
        {
            get => outerMarginSize;
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
        public virtual GridLength TitleHeightGridLength => new GridLength(TitleHight);
        public Thickness OuterMarginSizeThickness => new Thickness(OuterMarginSize);
        public Thickness InnerContentPadding { get; set; } = new Thickness(0);
        
        public BaseWindowViewModel(Window window)
        {
            this.window = window;            
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
        protected static Point GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }
        protected Point GetMousePosition2()
        {
            var position = Mouse.GetPosition(window);
            return new Point(position.X + window.Left, position.Y + window.Top);
        }
    }
}