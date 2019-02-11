using System;
using MIDE.API.Components;
using MIDE.Application;
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

            program.Kernel.FileManager = new WinFileManager();
            program.Kernel.SystemClipboard = WindowsClipboard.Instance;
            program.Kernel.Start();

            Menu menu = new Menu("menu");
            menu.AddItem(new MenuButton("file", -99));
            MenuGroup groupBasic = new MenuGroup("gr-file-file", -99);
            groupBasic.Add(new MenuButton("new", -99), null);
            groupBasic.Add(new MenuButton("file", -99), "new");
            groupBasic.Add(new MenuButton("folder", -98), "new");

            menu.AddItem(groupBasic);
            menu.AddItem("file", new MenuButton("exit", 99)
            {
                PressCommand = null
            });
            menu.AddItem(new MenuButton("edit", -98));
            menu.AddItem(new MenuButton("view", -97));
            menu.AddItem(new MenuButton("tools", 50));
            menu.AddItem(new MenuButton("help", 99));

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.WriteLine(program.Kernel.SystemClipboard.Pop(API.Services.DataFormat.FileDrop));
            Console.ReadKey();
            program.Kernel.Exit();
        }
    }
}