namespace NodeGraphs.Components
{
    public class DoubleJoint : NodeJoint<double>
    {
        public DoubleJoint(GraphNode parent, JointRole role, int maxConnections) : base(parent, role, maxConnections)
        {

        }

        public override object GetValue() => Value;
    }
}