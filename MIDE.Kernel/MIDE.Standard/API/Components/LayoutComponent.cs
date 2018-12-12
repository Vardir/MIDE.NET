using System.ComponentModel;
using MIDE.Standard.API.Measurements;

namespace MIDE.Standard.API.Components
{
    public abstract class LayoutComponent : IApplicationComponent, INotifyPropertyChanged
    {
        private bool isEnabled;
        private GridLength width;
        private GridLength height;
        private BoundingBox margin;
        private BoundingBox padding;
        private LayoutComponent parent;
        
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
        public string Id { get; }
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

        public LayoutComponent(string id)
        {
            Id = id;
            width = GridLength.Auto;
            height = GridLength.Auto;
        }

        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}