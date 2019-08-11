using MIDE.API;

namespace MIDE.Components
{
    /// <summary>
    /// The base class for all the application components that are required to be visually represented
    /// </summary>
    public abstract class LayoutComponent : ApplicationComponent, ICloneable<LayoutComponent>
    {
        private bool isEnabled = true;
        private Visibility visibility;
        private LayoutComponent parent;
        private ContextMenu currentContextMenu;

        public bool IsEnabled
        {
            get => isEnabled;
            set => SetAndNotify(value, ref isEnabled);
        }
        public Visibility Visibility
        {
            get => visibility;
            set => SetAndNotify(value, ref visibility);
        }
        public LayoutComponent Parent
        {
            get => parent;
            set => SetAndNotify(value, ref parent);
        }

        public LayoutComponent(string id) : base(id)
        {

        }

        public LayoutComponent Clone(string id)
        {
            var clone = CloneInternal(id);
            clone.isEnabled = isEnabled;
            clone.visibility = visibility;
            clone.parent = null;
            clone.currentContextMenu = currentContextMenu;

            return clone;
        }
        public LayoutComponent Clone() => Clone(Id);
        protected abstract LayoutComponent CloneInternal(string id);
    }
}