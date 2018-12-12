using System.Collections.Generic;

namespace MIDE.Standard.API.Components
{
    public abstract class LayoutContainer : LayoutComponent
    {
        public LayoutContainer(string id) : base(id) {}

        public abstract void AddChild(LayoutComponent component);
        public abstract void RemoveChild(string id);
        public abstract void RemoveChild(LayoutComponent component);

        public abstract bool Contains(string id);
        public abstract LayoutComponent Find(string id);
    }
}