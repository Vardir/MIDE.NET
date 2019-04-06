using System.ComponentModel;

namespace MIDE.API.Components
{
    /// <summary>
    /// The base class for all the application components that are required to be visually represented
    /// </summary>
    public abstract class LayoutComponent : ApplicationComponent, INotifyPropertyChanged, ICloneable<LayoutComponent>
    {
        private bool isEnabled = true;
        private Visibility visibility;
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

        }

        public LayoutComponent Clone(string id)
        {
            LayoutComponent clone = CloneInternal(id);
            clone.isEnabled = isEnabled;
            clone.visibility = visibility;
            clone.parent = null;
            clone.currentContextMenu = currentContextMenu;
            return clone;
        }
        public LayoutComponent Clone() => Clone(Id);
        protected abstract LayoutComponent CloneInternal(string id);

        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}