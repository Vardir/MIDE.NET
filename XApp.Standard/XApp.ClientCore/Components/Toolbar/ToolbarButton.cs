using Vardirsoft.Shared.MVVM;

namespace Vardirsoft.XApp.Components
{
    public class ToolbarButton : LayoutComponent, IToolBarItem
    {
        private BaseCommand pressCommand;

        public int Order { get; set; }
        public string Group { get; set; }

        public BaseCommand PressCommand
        {
            get => pressCommand;
            set => SetWithNotify(ref pressCommand, value, true);
        }

        public ToolbarButton(string id) : base(id) {}

        public void Press(object parameter) => PressCommand.Execute(parameter);

        protected override LayoutComponent CloneInternal(string id)
        {
            var clone = new ToolbarButton(id);
            clone.Order = Order;
            clone.Group = Group;
            clone.pressCommand = pressCommand;
            
            return clone;
        }
    }
}