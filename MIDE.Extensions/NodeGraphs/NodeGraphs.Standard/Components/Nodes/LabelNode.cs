namespace NodeGraphs.Components
{
    public class LabelNode : GraphNode
    {
        private string text;
        
        public string Text
        {
            get => text;
            set
            {
                if (value == text)
                    return;
                text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        public LabelNode(string id, string defaultText = "Label", IGraphNodeContentProvider contentProvider = null) : base(id, contentProvider)
        {
            Text = defaultText;
        }
    }
}