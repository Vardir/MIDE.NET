using System.ComponentModel;
using MIDE.API.Measurements;

namespace MIDE.API.Components
{
    /// <summary>
    /// The base class for all the application components that are required to be visually represented
    /// </summary>
    public abstract class LayoutComponent : ApplicationComponent, INotifyPropertyChanged
    {
        private bool isEnabled = true;
        private Visibility visibility;
        private GridLength width;
        private GridLength height;
        private BoundingBox margin;
        private BoundingBox padding;
        private LayoutComponent parent;
        private ContextMenu currentContextMenu;

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
        public virtual GridLength Width
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
        public virtual GridLength Height
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
            width = GridLength.Auto;
            height = GridLength.Auto;
        }

        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}