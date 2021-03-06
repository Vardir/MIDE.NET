﻿using MIDE.API.Commands;
using MIDE.API.Components;
using MIDE.API.Extensibility;
using MIDE.Application.Attrubites;

namespace MIDE.Kernel.ConsoleTest.Extensions
{
    [Dependency(Version = "0.6.2", Type = DependencyType.ApplicationKernel)]
    [ExtensionProperties(Version = "1.0.0")]
    public class TestExtension1 : AppExtension
    {
        public TestExtension1(string id, bool isEnabled) : base(id, isEnabled) { }

        protected override void RegisterMenuItems(IMenuConstructionContext context)
        {
            var newFile = new MenuButton("new-file", -99);
            newFile.PressCommand = new RelayCommand(() => System.Console.WriteLine("Creating new file"));
            context.AddItem("file", newFile);

            var testItem = new MenuButton("test", 0);
            testItem.PressCommand = new RelayCommand(() => System.Console.WriteLine("Test button pressed"));
            context.AddItem("file/open/test-block", testItem);
        }
        protected override void RegisterModules()
        {
            
        }
    }
}