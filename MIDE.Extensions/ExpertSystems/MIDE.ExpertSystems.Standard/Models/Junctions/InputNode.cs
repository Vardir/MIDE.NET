namespace MIDE.ExpertSystems.Models
{
    public abstract class InputNode<T> : Node
    {
        public T Value { get; protected set; }
        public OutputJunction<T> Output { get; set; }

        public void Set(T value)
        {
            Value = value;
        }

        public override void Pulse()
        {
            Output.Set(Value);
        }
    }
}