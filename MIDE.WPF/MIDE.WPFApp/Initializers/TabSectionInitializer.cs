using MIDE.Application;
using MIDE.API.Components;
using MIDE.Application.Initializers;

namespace MIDE.WPFApp.Initializers
{
    public class TabSectionInitializer : IApplicationInitializer
    {
        public void Execute(AppKernel appKernel)
        {
            appKernel.UIManager.AddSection(new TabSection("browsers"));
            appKernel.UIManager["browsers"].Insert(0, new Tab("file-explorer"));
            var tab = (appKernel.UIManager["browsers"].Find("file-explorer")) as Tab;
            var grid = new GridLayout("container");
            tab.ContentContainer = grid;
            grid.Columns.Add(new GridColumn() { Width = new API.Measurements.GridLength("*") });
            grid.Rows.Add(new GridRow() { Height = new API.Measurements.GridLength("50") });
            grid.Rows.Add(new GridRow() { Height = new API.Measurements.GridLength("50") });
            grid.AddChild(new Button("test-button"), 0, 0);
            grid.AddChild(new Button("test-button2"), 1, 0);

            appKernel.UIManager.AddSection(new TabSection("editors"));
            appKernel.UIManager.AddSection(new TabSection("properties"));
            appKernel.UIManager.AddSection(new TabSection("loggers"));
        }
    }
}