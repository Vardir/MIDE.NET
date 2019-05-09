using MIDE.API.Commands;
using MIDE.API.Components;
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
            //var newFile = new MenuButton("new-file", -99);
            //newFile.PressCommand = new RelayCommand(() => System.Console.WriteLine("Creating new file"));
            //context.AddItem("file", newFile);
        }
        protected override void RegisterModules()
        {

        }
    }
}
