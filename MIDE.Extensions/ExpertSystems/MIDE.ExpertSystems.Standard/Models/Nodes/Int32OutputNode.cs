namespace MIDE.ExpertSystems.Models
{
    public class Int32OutputNode : Node
    {
        public int Value { get; private set; }
        public InputJunction<int> Input { get; set; }

        public override void Pulse()
        {
            if (Input.IsSet)
                Value = Input.ConnectedJunction.Value;
        }
    }
}