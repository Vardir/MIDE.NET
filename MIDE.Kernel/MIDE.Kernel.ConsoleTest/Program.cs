using System;
using MIDE.Application;
using MIDE.Kernel.ConsoleTest.Extensions;
using MIDE.Application.Attrubites;

[assembly: ApplicationProperties("consoletest")]

namespace MIDE.Kernel.ConsoleTest
{
    public class Program
    {
        public AppKernel Kernel => AppKernel.Instance;

        private Program()
        {

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