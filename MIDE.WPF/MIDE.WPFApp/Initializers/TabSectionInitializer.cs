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
            grid.ColumnMargin = new API.Measurements.GridLength("5");
            grid.RowMargin = new API.Measurements.GridLength("5");
            grid.Columns.Add(new GridColumn() { Width = new API.Measurements.GridLength("*") });
            grid.Columns.Add(new GridColumn() { Width = new API.Measurements.GridLength("*") });
            grid.Rows.Add(new GridRow() { Height = new API.Measurements.GridLength("auto") });
            grid.Rows.Add(new GridRow() { Height = new API.Measurements.GridLength("50") });
            grid.Rows.Add(new GridRow() { Height = new API.Measurements.GridLength("50") });
            grid.Rows.Add(new GridRow() { Height = new API.Measurements.GridLength("50") });
            grid.AddChild(new Button("test-button"), 0, 1);
            grid.AddChild(new TextBox("test-tbox1", "Hello"), 0, 0);
            grid.AddChild(new TextBox("test-tbox2", "My"), 1, 0);
            grid.AddChild(new Label("test-label", "Label"), 1, 1);
            grid.AddChild(new TextBox("test-tbox3", "World"), 2, 0);
            grid.AddChild(new Button("test-button2"), 2, 1);

            appKernel.UIManager.AddSection(new TabSection("editors"));
            appKernel.UIManager.AddSection(new TabSection("properties"));
            appKernel.UIManager.AddSection(new TabSection("loggers"));
        }
    }
}