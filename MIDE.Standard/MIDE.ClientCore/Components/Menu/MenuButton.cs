using MIDE.API;

namespace MIDE.Components
{
    public class MenuButton : MenuItem, IButton
    {
        public ICommand PressCommand { get; set; }
        
        public MenuButton(string id, short ordinalIndex) : base(id, ordinalIndex)
        {

        }
        public MenuButton(string id, string group, short ordinalIndex) : base(id, group, ordinalIndex)
        {
            
        }

        public void Press(object parameter)
        {
            if (ChildCount > 0)
                return;
            if (PressCommand.CanExecute(parameter))
                PressCommand.Execute(parameter);
        }
        
        protected override MenuItem Create(string id, short ordinalIndex, string group)
        {
            MenuButton clone = new MenuButton(id, ordinalIndex);
            clone.PressCommand = PressCommand;
            return clone;
        }
    }
}