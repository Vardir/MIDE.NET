using System.Collections.Generic;

namespace MIDE.ExpertSystems.Models
{
    public class OutputJunction<T> : Junction<T>
    {
        public T Value { get; private set; }
        public List<InputJunction<T>> ConnectedJunctions { get; } 

        public OutputJunction(Node parent) : base(parent)
        {
            ConnectedJunctions = new List<InputJunction<T>>();
        }

        public override void Set()
        {
            IsSet = true;
            foreach (var junction in ConnectedJunctions)
            {
                junction.Set();
            }
        }
        public void Set(T value)
        {
            IsSet = true;
            Value = value;
            foreach (var junction in ConnectedJunctions)
            {
                junction.Set();
            }
        }

        public override void Reset()
        {
            IsSet = false;
        }
    }
}