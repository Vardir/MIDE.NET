using MIDE.Application;
using MIDE.API.Components;
using MIDE.Application.Initializers;

namespace MIDE.WPFApp.Initializers
{
    public class TabSectionInitializer : IApplicationInitializer
    {
        public void Execute(AppKernel appKernel)
        {
            appKernel.UIManager.AddTabSection(new TabSection("browsers"));

            var expl = new FileExplorer("file-explorer");
            appKernel.UIManager.AddTab(expl, "browsers");
            appKernel.UIManager["browsers"].SelectedIndex = 0;

            appKernel.UIManager.AddTabSection(new TabSection("editors"));
            appKernel.UIManager.AddTabSection(new TabSection("properties"));
            appKernel.UIManager.AddTabSection(new TabSection("loggers"));
        }
    }
}