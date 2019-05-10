namespace NodeGraphs.Components
{
    public class Int32ConstantNode : ConstantNode<int>
    {
        public Int32ConstantNode(string id, int defaultValue = 0, IGraphNodeContentProvider contentProvider = null) : base(id, defaultValue, contentProvider)
        {
            Output = new Int32Joint(this, JointRole.Output, -1);
        }
    }
}