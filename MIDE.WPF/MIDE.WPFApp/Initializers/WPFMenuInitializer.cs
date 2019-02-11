//#define USE_BLOCKS
#define DONT_USE_BLOCKS

using MIDE.Application;
using MIDE.API.Commands;
using MIDE.API.Components;
using MIDE.Application.Initializers;

namespace MIDE.WPFApp.Initializers
{
    public class ApplicationMenuInitializer : BaseMenuInitializer
    {
        public ApplicationMenuInitializer(AppKernel appKernel) : base(appKernel) { }

        protected override void PopulateMenu(IMenuConstructionContext context)
        {
            context.AddItem(new MenuButton("file", -99));

#if USE_BLOCKS
            MenuGroup groupBasic = new MenuGroup("gr-file-file", -99);
            groupBasic.Add(new MenuButton("new", -99), null);
            groupBasic.Add(new MenuButton("file", -99), "new");
            groupBasic.Add(new MenuButton("folder", -98), "new");

            context.AddItem("file", groupBasic);
            context.AddItem("file", new MenuButton("exit", 99)
            {
                PressCommand = new RelayCommand(appKernel.Exit)
            });
#endif

#if DONT_USE_BLOCKS
            context.AddItem("file", new MenuSplitter("split-exit", 98));
            context.AddItem("file", new MenuButton("exit", 99)
            {
                PressCommand = new RelayCommand(appKernel.Exit)
            });
            context.AddItem("file", new MenuButton("new", -99));
            context.AddItem("file/new", new MenuButton("file", -99));
            context.AddItem("file/new", new MenuButton("folder", -98));
#endif
            context.AddItem(new MenuButton("edit", -98));
            context.AddItem(new MenuButton("view", -97));
            context.AddItem(new MenuButton("tools", 50));
            context.AddItem(new MenuButton("help", 99));
        }
    }
}