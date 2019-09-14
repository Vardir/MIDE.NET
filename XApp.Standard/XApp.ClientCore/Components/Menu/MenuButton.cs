using XApp.Commands;

namespace XApp.Components
{
    public class MenuButton : MenuItem
    {
        public BaseCommand PressCommand { get; set; }
        
        public MenuButton(string id, short ordinalIndex) : base(id, ordinalIndex)
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

        protected override MenuItem Create(string id, short ordinalIndex, string group)
        {
            var clone = new MenuButton(id, ordinalIndex);
            clone.PressCommand = PressCommand;
            
            return clone;
        }
    }
}