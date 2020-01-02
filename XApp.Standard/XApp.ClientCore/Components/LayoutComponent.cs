using System.Diagnostics;

using Vardirsoft.XApp.API;

namespace Vardirsoft.XApp.Components
{
    /// <summary>
    /// The base class for all the application components that are required to be visually represented
    /// </summary>
    public abstract class LayoutComponent : ApplicationComponent, ICloneable<LayoutComponent>
    {
        private bool _isEnabled = true;
        private Visibility _visibility;
        private LayoutComponent _parent;
        private ContextMenu _currentContextMenu;

        public bool IsEnabled
        {
            [DebuggerStepThrough]
            get => _isEnabled;
            
            [DebuggerStepThrough]
            set => SetWithNotify(ref _isEnabled, value);
        }
        public Visibility Visibility
        {
            [DebuggerStepThrough]
            get => _visibility;
            
            [DebuggerStepThrough]
            set => SetWithNotify(ref _visibility, value);
        }
        public LayoutComponent Parent
        {
            [DebuggerStepThrough]
            get => _parent;
            
            [DebuggerStepThrough]
            set => SetWithNotify(ref _parent, value, true);
        }

        public LayoutComponent(string id) : base(id)
        {

        }

        public LayoutComponent Clone(string id)
        {
            var clone = CloneInternal(id);
            clone._isEnabled = _isEnabled;
            clone._visibility = _visibility;
            clone._parent = null;
            clone._currentContextMenu = _currentContextMenu;

            return clone;
        }
        
        [DebuggerStepThrough]
        public LayoutComponent Clone() => Clone(Id);
        
        protected abstract LayoutComponent CloneInternal(string id);
    }
}