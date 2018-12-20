using MIDE.Standard.API.Components;
using MIDE.Standard.API.Extensibility;

namespace MIDE.Kernel.ConsoleTest.Extensions
{
    public class TestExtension1 : AppExtension
    {
        public TestExtension1(string id) : base(id) { }

        protected override void RegisterMenuItems(IMenuConstructionContext context)
        {
            var exit = new MenuButton("exit");
            exit.Pressed += Kernel.Exit;
            context.AddItem("/", exit);

            var newFile = new MenuButton("new-file");
            newFile.Pressed += () => System.Console.WriteLine("Creating new file");
            context.AddItem("file", newFile);

            var testItem = new MenuButton("test");
            testItem.Pressed += () => System.Console.WriteLine("Test button pressed");
            context.AddItem("file/open/test-block", testItem);
        }
    }
}