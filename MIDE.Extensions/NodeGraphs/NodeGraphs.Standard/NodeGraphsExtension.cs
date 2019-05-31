using MIDE.API.Commands;
using MIDE.API.Components;
using NodeGraphs.Components;
using MIDE.API.Extensibility;

namespace NodeGraphs
{
    public class NodeGraphsExtension : AppExtension
    {
        public NodeGraphsExtension(string id, bool isEnabled) : base(id, isEnabled) { }

        protected override void RegisterMenuItems(IMenuConstructionContext context)
        {
            context.AddItem("view", new MenuButton("node-graph", 0)
            {
                Caption = "(ng:node-graph)",
                PressCommand = new RelayCommand(() =>
                {
                    NodeGraphTab tab = new NodeGraphTab("nodes-graph");
                    tab.Header = "(ng:node-graph)";
                    Kernel.UIManager.AddTab(tab, "editors");
                })
            });
        }
        protected override void RegisterModules()
        {

        }
    }
}
