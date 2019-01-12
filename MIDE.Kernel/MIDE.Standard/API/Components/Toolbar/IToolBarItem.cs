namespace MIDE.API.Components
{
    public interface IToolBarItem
    {
        int Order { get; }
        string Id { get; }
        string Group { get; }
    }
}