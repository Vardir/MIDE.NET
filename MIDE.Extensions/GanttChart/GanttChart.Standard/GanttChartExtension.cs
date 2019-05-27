using MIDE.API.Components;
using MIDE.API.Extensibility;

namespace GanttChart
{
    public class GanttChartExtension : AppExtension
    {
        public GanttChartExtension(string id, bool isEnabled) : base(id, isEnabled)
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