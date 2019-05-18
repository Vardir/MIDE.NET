using MIDE.API.Components;
using MIDE.API.Extensibility;
using MIDE.Application.Attrubites;

namespace GanttChart
{
    [Dependency(Version = "0.7.1", Type = DependencyType.ApplicationKernel)]
    [ExtensionProperties(Version = "1.0.0")]
    public class GanttChartExtension : AppExtension
    {
        public GanttChartExtension(string id) : base(id)
        {

        }

        protected override void RegisterMenuItems(IMenuConstructionContext context)
        {
            //context.AddItem("path", new MenuButton("button", 0));
        }
        protected override void RegisterModules()
        {
            //RegisterModule(new TModule(x));
        }
    }
}