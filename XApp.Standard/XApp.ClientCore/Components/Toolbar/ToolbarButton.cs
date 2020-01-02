using System.Diagnostics;

using Vardirsoft.Shared.MVVM;

namespace Vardirsoft.XApp.Components
{
    public class ToolbarButton : LayoutComponent, IToolBarItem
    {
        private BaseCommand _pressCommand;

        public int Order { get; set; }
        
        public string Group { get; set; }

        public BaseCommand PressCommand
        {
            [DebuggerStepThrough]
            get => _pressCommand;
            
            [DebuggerStepThrough]
            set => SetWithNotify(ref _pressCommand, value, true);
        }

        public ToolbarButton(string id) : base(id) {}

        [DebuggerStepThrough]
        public void Press(object parameter) => PressCommand.Execute(parameter);

        protected override LayoutComponent CloneInternal(string id)
        {
            var clone = new ToolbarButton(id) 
            {
                Order = Order, 
                Group = Group, 
                _pressCommand = _pressCommand
            };

            return clone;
        }
    }
}