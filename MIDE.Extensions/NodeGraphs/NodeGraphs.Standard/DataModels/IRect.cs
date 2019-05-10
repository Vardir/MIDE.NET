namespace NodeGraphs.DataModels
{
    public interface IRect
    {
        int LeftEdge { get; }
        int RightEdge { get; }
        int TopEdge { get; }
        int BottomEdge { get; }

        int GetWidth();
        int GetHeight();
    }
}