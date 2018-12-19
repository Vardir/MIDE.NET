using System;
using MIDE.Standard.Application;
using MIDE.Standard.Application.Attrubites;

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

            Console.WriteLine("Press any key to continue...");
            program.Kernel.Exit();
        }
    }
}