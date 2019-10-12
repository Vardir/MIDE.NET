using Vardirsoft.XApp.API;

namespace Vardirsoft.XApp.Components
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
            set => SetWithNotify(ref isEnabled, value);
        }
        public Visibility Visibility
        {
            get => visibility;
            set => SetWithNotify(ref visibility, value);
        }
        public LayoutComponent Parent
        {
            get => parent;
            set => SetWithNotify(ref parent, value, true);
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