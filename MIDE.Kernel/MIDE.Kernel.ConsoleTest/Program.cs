﻿using System;
using MIDE.Application;
using MIDE.API.Components;
using MIDE.Application.Attrubites;
using MIDE.FileSystem;
using MIDE.API.Extensibility;
using System.Threading;

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
            Console.ReadKey();
            FileManager.Instance = new WinFileManager();
            Program program = new Program();

            var module = new AModule();
            module.Execute("tam 4", new BListener());
            module.Execute("tam 1", new BListener());
            module.Execute("dam 3", new BListener());
            module.Execute("dam 2", new BListener());

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.WriteLine(program.Kernel.SystemClipboard.Pop(API.Services.DataFormat.FileDrop));
            Console.ReadKey();
            program.Kernel.Exit();
        }

        private class AModule : Module
        {
            public AModule() : base("amodule")
            {

            }

            protected override ModuleJob ExecuteCommand(object parameter, IModuleExecutionListener listener)
            {
                int delay = int.Parse((parameter as string).Substring(4));
                listener.ReceiveStatus(0, $"working on {parameter}");
                Thread.Sleep(delay * 1000);
                listener.ReceiveStatus(50, $"still working on {parameter}");
                Thread.Sleep(delay * 1000);
                return new ModuleJob($"done with {parameter}", listener);
            }
        }

        private class BListener : IModuleExecutionListener
        {
            public void ReceiveResult(object context)
            {
                Console.WriteLine($"Received result: {context}");
            }

            public void ReceiveStatus(int completition, object context)
            {
                Console.WriteLine($"Job completed at {completition}% [{context}]");
            }
        }
    }
}