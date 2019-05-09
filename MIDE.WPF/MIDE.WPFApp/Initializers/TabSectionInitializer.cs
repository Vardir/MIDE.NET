using MIDE.Application;
using MIDE.API.Components;
using MIDE.Application.Initializers;

namespace MIDE.WPF.Initializers
{
    public class TabSectionInitializer : IApplicationInitializer
    {
        public void Execute(AppKernel appKernel)
        {
            appKernel.UIManager.AddTabSection(new TabSection("browsers"));            
            appKernel.UIManager.AddTabSection(new TabSection("editors"));
            appKernel.UIManager.AddTabSection(new TabSection("properties"));
            appKernel.UIManager.AddTabSection(new TabSection("loggers"));
        }
    }
}