namespace NodeGraphs.Components
{
    public class TextConstantNode : ConstantNode<string>
    {
        public TextConstantNode(string id, string defaultValue = "", IGraphNodeContentProvider contentProvider = null) : base(id, defaultValue, contentProvider)
        {
            Output = new StringJoint(this, JointRole.Output, -1);
        }
    }
}