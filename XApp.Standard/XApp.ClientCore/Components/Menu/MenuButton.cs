using Vardirsoft.Shared.MVVM;

namespace Vardirsoft.XApp.Components
{
    public class MenuButton : MenuItem
    {
        public BaseCommand PressCommand { get; set; }
        
        public MenuButton(string id, int ordinalIndex) : base(id, ordinalIndex)
        {

        }
        public MenuButton(string id, string group, short ordinalIndex) : base(id, group, ordinalIndex)
        {
            
        }

        public void Press(object parameter)
        {
            if (ChildCount == 0)
                PressCommand.Execute(parameter);
        }

        protected override MenuItem Create(string id, int ordinalIndex, string group)
        {
            var clone = new MenuButton(id, ordinalIndex) { PressCommand = PressCommand };

            return clone;
        }
    }
}