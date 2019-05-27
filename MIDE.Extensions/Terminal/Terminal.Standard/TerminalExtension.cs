using MIDE.API.Commands;
using MIDE.API.Components;
using Terminal.Components;
using MIDE.API.Extensibility;

namespace Terminal
{
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