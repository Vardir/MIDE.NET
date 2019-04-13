using System;
using MIDE.API.Components;
using MIDE.Application;
using MIDE.Application.Attrubites;
using MIDE.Schemes.JSON;
using Newtonsoft.Json;

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

            program.Kernel.FileManager = new WinFileManager();
            //program.Kernel.SystemClipboard = WindowsClipboard.Instance;
            //program.Kernel.Start();
            CreateProjectDialogBox dialogBox = new CreateProjectDialogBox("create");
            
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.WriteLine(program.Kernel.SystemClipboard.Pop(API.Services.DataFormat.FileDrop));
            Console.ReadKey();
            program.Kernel.Exit();
        }
    }
}