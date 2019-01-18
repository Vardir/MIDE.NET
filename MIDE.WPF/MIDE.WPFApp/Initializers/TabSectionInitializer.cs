using MIDE.Helpers;
using MIDE.Application;
using MIDE.API.Components;
using MIDE.API.Measurements;
using MIDE.Application.Initializers;

namespace MIDE.WPFApp.Initializers
{
    public class TabSectionInitializer : IApplicationInitializer
    {
        public void Execute(AppKernel appKernel)
        {
            appKernel.UIManager.AddTabSection(new TabSection("browsers"));
            var tab = new Tab("test-tab");
            appKernel.UIManager.AddTab(tab, "browsers");
            var grid = new GridLayout("container");
            tab.ContentContainer = grid;
            grid.ColumnMargin = new GridLength("5");
            grid.RowMargin = new GridLength("5");
            grid.Columns.AddRange(new [] {
                new GridColumn() { Width = new GridLength("*") },
                new GridColumn() { Width = new GridLength("*") }
            });
            grid.Rows.AddRange(new[] {
                new GridRow() { Height = new GridLength("auto") },
                new GridRow() { Height = new GridLength("50") },
                new GridRow() { Height = new GridLength("50") },
                new GridRow() { Height = new GridLength("50") }
            });
            grid.AddChild(new Button("test-button"), 0, 1);
            grid.AddChild(new TextBox("test-tbox1", "Hello"), 0, 0);
            grid.AddChild(new TextBox("test-tbox2", "My"), 1, 0);
            grid.AddChild(new Label("test-label", "Label"), 1, 1);
            grid.AddChild(new TextBox("test-tbox3", "World"), 2, 0);
            grid.AddChild(new Button("test-button2"), 2, 1);

            var tab2 = new Tab("test-tab-2");
            appKernel.UIManager.AddTab(tab2, "browsers");
            var stack = new StackPanel("container");
            tab2.ContentContainer = stack;
            stack.AddChild(new Label("lbl", "hello world"));

            var expl = new FileExplorer("file-explorer");
            appKernel.UIManager.AddTab(expl, "browsers");

            appKernel.UIManager.AddTabSection(new TabSection("editors"));
            appKernel.UIManager.AddTabSection(new TabSection("properties"));
            appKernel.UIManager.AddTabSection(new TabSection("loggers"));
        }
    }
}