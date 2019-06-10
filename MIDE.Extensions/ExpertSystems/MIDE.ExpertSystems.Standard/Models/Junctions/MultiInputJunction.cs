using System.Collections.Generic;

namespace MIDE.ExpertSystems.Models
{
    public class MultiInputJunction<T> : InputJunction<T>
    {
        private int connectionsSet;

        public List<OutputJunction<T>> ConnectedJunctions { get; } 

        public MultiInputJunction(Node parent) : base(parent)
        {
            ConnectedJunctions = new List<OutputJunction<T>>();
        }

        public override void Set()
        {
            if (connectionsSet < ConnectedJunctions.Count)            
                connectionsSet++;
            if (ConnectedJunctions.Count == connectionsSet)
            {
                IsSet = true;
                Parent.Pulse();
            }
        }
        public override void Reset()
        {
            IsSet = false;
            connectionsSet = 0;
        }
    }
}