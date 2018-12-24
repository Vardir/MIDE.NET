using MIDE.Standard.API.Commands;
using MIDE.Standard.API.Components;
using MIDE.Standard.API.Extensibility;

namespace MIDE.Kernel.ConsoleTest.Extensions
{
    public class TestExtension1 : AppExtension
    {
        public TestExtension1(string id) : base(id) { }

        protected override void RegisterMenuItems(IMenuConstructionContext context)
        {
            var newFile = new MenuButton("new-file", -99);
            newFile.PressCommand = new RelayCommand(() => System.Console.WriteLine("Creating new file"));
            context.AddItem("file", newFile);

            var testItem = new MenuButton("test", 0);
            testItem.PressCommand = new RelayCommand(() => System.Console.WriteLine("Test button pressed"));
            context.AddItem("file/open/test-block", testItem);
        }
    }
}