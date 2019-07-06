namespace MIDE.API
{
    /// <summary>
    /// An interface that indicates that item can be ordered in collection
    /// </summary>
    public interface IOrderable
    {
        /// <summary>
        /// Ordinal index identifying item positioning in collection
        /// </summary>
        short OrdinalIndex { get; }
    }
}