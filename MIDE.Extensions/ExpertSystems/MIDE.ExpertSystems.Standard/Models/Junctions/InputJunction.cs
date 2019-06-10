namespace MIDE.ExpertSystems.Models
{
    public class InputJunction<T> : Junction<T>
    {
        public OutputJunction<T> ConnectedJunction { get; set; }

        public InputJunction(Node parent) : base(parent)
        {

        }

        public override void Set()
        {
            IsSet = true;
            ConnectedJunction.Set();
        }
        public override void Reset()
        {
            IsSet = false;
        }
    }
}