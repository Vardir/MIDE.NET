using System.Collections.Generic;

namespace MIDE.Standard.API.Components
{
    public interface ILayoutContainer : ILayoutComponent
    {
        IList<ILayoutComponent> Children { get; }

        void AddChild(ILayoutComponent component);
        void RemoveChild(string id);
        void RemoveChild(ILayoutComponent component);

        ILayoutComponent Find(string id);
    }
}