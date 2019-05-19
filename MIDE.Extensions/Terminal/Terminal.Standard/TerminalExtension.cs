using MIDE.API.Commands;
using MIDE.API.Components;
using Terminal.Components;
using MIDE.API.Extensibility;
using MIDE.Application.Attrubites;

namespace Terminal
{
    [Dependency(Version = "0.7.1", Type = DependencyType.ApplicationKernel)]
    [ExtensionProperties(Version = "1.0.0")]
    public class TerminalExtension : AppExtension
    {
        public TerminalExtension(string id, bool isEnabled) : base(id, isEnabled)
        {

        }

        protected override void RegisterMenuItems(IMenuConstructionContext context)
        {
            context.AddItem("view", new MenuButton("terminal", 50)
            {
                PressCommand = new RelayCommand(OpenTerminal)
            });
        }
        protected override void RegisterModules()
        {
            RegisterModule(new TerminalModule());
        }

        private void OpenTerminal()
        {
            Kernel.UIManager.GetOrAddTab("loggers", () => new TerminalTab("terminal", this));
        }
    }
}