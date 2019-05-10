namespace NodeGraphs.Components
{
    public class StringJoint : NodeJoint<string>
    {
        public StringJoint(GraphNode parent, JointRole role, int maxConnections) : base(parent, role, maxConnections)
        {

        }

        public override object GetValue() => Value;
    }
}