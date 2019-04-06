using System;

namespace MIDE.API.Components
{
    public abstract class LayoutContainer : LayoutComponent
    {
        /// <summary>
        /// Indicates that the container does not accept adding/removing child elements
        /// </summary>
        public bool IsSealed { get; protected set; }

        public LayoutContainer(string id) : base(id)
        {
            
        }

        public void AddChild(LayoutComponent component)
        {
            if (IsSealed)
                throw new InvalidOperationException("Can not add any child elements into a sealed control");
            AddChild_Impl(component);
        }
        public void RemoveChild(string id)
        {
            if (IsSealed)
                throw new InvalidOperationException("Can not remove child elements from a sealed control");
            RemoveChild_Impl(id);
        }
        public void RemoveChild(LayoutComponent component)
        {
            if (IsSealed)
                throw new InvalidOperationException("Can not remove child elements from a sealed control");
            RemoveChild_Impl(component);
        }

        public abstract bool Contains(string id);
        public abstract LayoutComponent Find(string id);

        protected abstract void AddChild_Impl(LayoutComponent component);
        protected abstract void RemoveChild_Impl(string id);
        protected abstract void RemoveChild_Impl(LayoutComponent component);

        protected virtual void InitializeComponents() { }
    }
}