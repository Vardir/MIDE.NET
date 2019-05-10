namespace NodeGraphs.Components
{
    public class Int32Joint : NodeJoint<int>
    {
        public Int32Joint(GraphNode parent, JointRole role, int maxConnections) : base(parent, role, maxConnections)
        {

        }

        public override object GetValue() => Value;
    }
}