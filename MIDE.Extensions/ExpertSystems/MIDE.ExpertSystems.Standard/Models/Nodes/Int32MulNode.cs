namespace MIDE.ExpertSystems.Models
{
    public class Int32MulNode : Node
    {
        public MultiInputJunction<int> Input { get; set; }
        public OutputJunction<int> Output { get; set; }

        public override void Pulse()
        {
            if (Input.IsSet)
            {
                int result = 1;
                foreach (var item in Input.ConnectedJunctions)
                {
                    result *= item.Value;
                }
                Output.Set(result);
                Input.Reset();
            }
        }
    }
}