namespace NodeGraphs.DataModels
{
    public interface IBounds
    {
        int LeftBound { get; }
        int RightBound { get; }
        int TopBound { get; }
        int BottomBound { get; }
    }
}