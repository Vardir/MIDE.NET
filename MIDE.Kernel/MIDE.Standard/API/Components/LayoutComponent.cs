using System.ComponentModel;
using MIDE.API.Measurements;

namespace MIDE.API.Components
{
    /// <summary>
    /// The base class for all the application components that are required to be visually represented
    /// </summary>
    public abstract class LayoutComponent : ApplicationComponent, INotifyPropertyChanged
    {
        #region Backing fields
        private bool isEnabled = true;
        private Visibility visibility;
        private VerticalAlignment verticalAlignment;
        private HorizontalAlignment horizontalAlignment;
        private double width;
        private double height;
        private double minWidth;
        private double minHeight;
        private double maxWidth;
        private double maxHeight;
        private BoundingBox margin;
        private BoundingBox padding;
        private LayoutComponent parent;
        private ContextMenu currentContextMenu; 
        #endregion

        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                if (isEnabled == value)
                    return;
                isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }
        public Visibility Visibility
        {
            get => visibility;
            set
            {
                if (value == visibility)
                    return;
                visibility = value;
                OnPropertyChanged(nameof(Visibility));
            }
        }
        public VerticalAlignment VerticalAlignment
        {
            get => verticalAlignment;
            set
            {
                if (verticalAlignment == value)
                    return;
                verticalAlignment = value;
                OnPropertyChanged(nameof(VerticalAlignment));
            }
        }
        public HorizontalAlignment HorizontalAlignment
        {
            get => horizontalAlignment;
            set
            {
                if (horizontalAlignment == value)
                    return;
                horizontalAlignment = value;
                OnPropertyChanged(nameof(HorizontalAlignment));
            }
        }
        public virtual double Width
        {
            get => width;
            set
            {
                if (width == value)
                    return;
                width = value;
                OnPropertyChanged(nameof(Width));
            }
        }
        public virtual double Height
        {
            get => height;
            set
            {
                if (height == value)
                    return;
                height = value;
                OnPropertyChanged(nameof(Height));
            }
        }
        public virtual double MinWidth
        {
            get => minWidth;
            set
            {
                if (value == minWidth)
                    return;
                minWidth = value;
                OnPropertyChanged(nameof(MinWidth));
            }
        }
        public virtual double MinHeight
        {
            get => minHeight;
            set
            {
                if (value == minHeight)
                    return;
                minHeight = value;
                OnPropertyChanged(nameof(MinHeight));
            }
        }
        public virtual double MaxHeight
        {
            get => maxHeight;
            set
            {
                if (value == maxHeight)
                    return;
                maxHeight = value;
                OnPropertyChanged(nameof(MaxHeight));
            }
        }
        public virtual double MaxWidth
        {
            get => maxWidth;
            set
            {
                if (value == maxWidth)
                    return;
                maxWidth = value;
                OnPropertyChanged(nameof(MaxWidth));
            }
        }
        public virtual BoundingBox Margin
        {
            get => margin;
            set
            {
                if (margin == value)
                    return;
                margin = value;
                OnPropertyChanged(nameof(Margin));
            }
        }
        public virtual BoundingBox Padding
        {
            get => padding;
            set
            {
                if (padding == value)
                    return;
                padding = value;
                OnPropertyChanged(nameof(Padding));
            }
        }
        public LayoutComponent Parent
        {
            get => parent;
            set
            {
                if (parent == value)
                    return;
                parent = value;
                OnPropertyChanged(nameof(Parent));
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        public LayoutComponent(string id) : base(id)
        {
            Width = double.NaN;
            Height = double.NaN;
            MaxHeight = double.PositiveInfinity;
            MaxWidth = double.PositiveInfinity;
        }

        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}