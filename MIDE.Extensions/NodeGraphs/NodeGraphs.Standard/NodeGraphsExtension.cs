using MIDE.API.Commands;
using MIDE.API.Components;
using NodeGraphs.Components;
using MIDE.API.Extensibility;
using MIDE.Application.Attrubites;

namespace NodeGraphs
{
    [Dependency(Version = "0.7.1", Type = DependencyType.ApplicationKernel)]
    [ExtensionProperties(Version = "1.0.0")]
    public class NodeGraphsExtension : AppExtension
    {
        public NodeGraphsExtension(string id) : base(id) { }

        protected override void RegisterMenuItems(IMenuConstructionContext context)
        {
            context.AddItem("view", new MenuButton("node-graph", 0)
            {
                PressCommand = new RelayCommand(() =>
                {
                    NodeGraphTab tab = new NodeGraphTab("nodes-graph");
                    Kernel.UIManager.AddTab(tab, "editors");
                })
            });
        }
        protected override void RegisterModules()
        {

        }
    }
}
