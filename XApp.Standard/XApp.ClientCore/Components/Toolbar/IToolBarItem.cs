using Vardirsoft.XApp.API;

namespace Vardirsoft.XApp.Components
{
    public interface IToolBarItem : ICloneable<LayoutComponent>
    {
        int Order { get; }
        string Id { get; }
        string Group { get; }
    }
}