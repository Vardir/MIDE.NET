using System;
using MIDE.Application;
using MIDE.API.Components;
using MIDE.Application.Attrubites;
using MIDE.Kernel.ConsoleTest.Extensions;

[assembly: ApplicationProperties("consoletest")]

namespace MIDE.Kernel.ConsoleTest
{
    public class Program
    {
        public AppKernel Kernel => AppKernel.Instance;

        private Program()
        {
            Kernel.UIManager.AddTabSection(new TabSection("browsers"));
            var expl = new FileExplorer("file-explorer");
            expl.Show("C:/");
            expl.Show("C:/Users");
            expl.Show("E:/");
            (expl.TabToolbar.Find("back") as ToolbarButton).Press(null);
            (expl.TabToolbar.Find("back") as ToolbarButton).Press(null);
            Kernel.UIManager.AddTab(expl, "browsers");
        }

        public static void Main(string[] args)
        {
            Program program = new Program();

            program.Kernel.FileManager = new WinFileManager();
            program.Kernel.Start();
            program.Kernel.RegisterExtension(new TestExtension1("test-ext1"));

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            program.Kernel.Exit();
        }
    }
}