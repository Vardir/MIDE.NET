using System;
using MIDE.Application;
using MIDE.API.Components;
using MIDE.Application.Attrubites;

[assembly: ApplicationProperties("consoletest")]

namespace MIDE.Kernel.ConsoleTest
{
    public class Program
    {
        public AppKernel Kernel => AppKernel.Instance;

        private Program() {}

        [STAThread]
        public static void Main(string[] args)
        {
            Program program = new Program();

            TabSection section = new TabSection("section");
            FileExplorer explorer = new FileExplorer("file-explorer");
            section.AddChild(explorer);
            explorer.Close();
            section.AddChild(new FileExplorer("file-explorer"));

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.WriteLine(program.Kernel.SystemClipboard.Pop(API.Services.DataFormat.FileDrop));
            Console.ReadKey();
            program.Kernel.Exit();
        }
    }
}