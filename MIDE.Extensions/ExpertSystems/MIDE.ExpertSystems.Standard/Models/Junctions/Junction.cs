namespace MIDE.ExpertSystems.Models
{
    public abstract class Junction<T>
    {
        public bool IsSet { get; protected set; }
        public Node Parent { get; }

        public Junction(Node parent)
        {
            Parent = parent;
        }

        public abstract void Set();
        public abstract void Reset();
    }
}