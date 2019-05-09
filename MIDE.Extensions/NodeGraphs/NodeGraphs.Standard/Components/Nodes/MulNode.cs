using System;

namespace NodeGraphs.Components
{
    public class MulNode : GraphNode
    {
        public DoubleJoint Input { get; }
        public DoubleJoint Output { get; }

        public MulNode(string id, IGraphNodeContentProvider contentProvider = null) : base(id, contentProvider)
        {
            Input = new DoubleJoint(this, JointRole.Input, -1);
            Output = new DoubleJoint(this, JointRole.Output, -1);

            Input.Pulsed += OnInputPulsed;
        }

        protected void OnInputPulsed()
        {
            if (Input.ConnectionsCount == 0)
                return;
            double result = 1;
            int index = 1;
            foreach (GraphConnection connection in Input.Connections)
            {
                object storedValue = connection.OutputJoint.GetValue();
                double value = 1;
                try
                {
                    value = Convert.ToDouble(storedValue);
                }
                catch
                {
                    AddError($"Not a number sent from connection [{index}]", storedValue);
                }
                result *= value;
                index++;
            }
            Output.Push(result);
        }
    }
}