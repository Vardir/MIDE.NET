using MIDE.API.Components;

namespace NodeGraphs.Components
{
    public class NodeGraphTab : Tab
    {
        private GraphCanvas canvas;

        public GraphCanvas Canvas
        {
            get => canvas;
            set
            {
                if (value == canvas)
                    return;
                canvas = value;
                OnPropertyChanged(nameof(Canvas));
            }
        }

        public NodeGraphTab(string id, bool allowDuplicates = false) : base(id, allowDuplicates)
        {
            Canvas = new GraphCanvas("canvas");
        }

        protected override Tab Create(string id, Toolbar toolbar, bool allowDuplicates)
        {
            NodeGraphTab clone = new NodeGraphTab(id, allowDuplicates);
            foreach (var item in toolbar.Items)
            {
                clone.TabToolbar.AddChild(item.Clone());
            }
            return clone;
        }
    }
}