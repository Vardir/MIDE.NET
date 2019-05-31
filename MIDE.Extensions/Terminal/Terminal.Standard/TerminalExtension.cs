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
                Caption = "(t:terminal)",
                PressCommand = new RelayCommand(() =>
                {
                    Kernel.UIManager.GetOrAddTab("loggers", () => new TerminalTab("terminal", this)
                                                                  { Header = "(t:terminal)" });
                })
            });
        }
        protected override void RegisterModules()
        {
            RegisterModule(new TerminalModule());
        }
    }
}