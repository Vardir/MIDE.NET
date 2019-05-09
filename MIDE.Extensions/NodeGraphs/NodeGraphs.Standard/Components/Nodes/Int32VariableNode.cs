using System;
using System.Linq;

namespace NodeGraphs.Components
{
    public class Int32VariableNode : GraphNode
    {
        private int value;

        public int Value
        {
            get => value;
            set
            {
                if (this.value == value)
                    return;
                this.value = value;
                OnPropertyChanged(nameof(Value));
            }
        }
        public Int32Joint Input { get; }

        public Int32VariableNode(string id, IGraphNodeContentProvider contentProvider = null) : base(id, contentProvider)
        {
            Input = new Int32Joint(this, JointRole.Input, 1);

            Input.Pulsed += OnInputPulsed;
        }

        protected void OnInputPulsed()
        {
            if (Input.ConnectionsCount == 0)
                return;
            GraphConnection connection = Input.Connections.First();
            object storedValue = connection.OutputJoint.GetValue();
            try
            {
                Value = Convert.ToInt32(storedValue);
            }
            catch
            {
                AddError("Expected a number as input", storedValue);
            }
        }
    }
}