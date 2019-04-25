using System.Linq;
using MIDE.API.Commands;
using MIDE.Helpers;

namespace MIDE.API.Components
{
    public class MenuButton : MenuItem, IButton
    {
        public ICommand PressCommand { get; set; }
        
        public MenuButton(string id, short ordinalIndex, bool allowDuplicates = false) : base(id, ordinalIndex, allowDuplicates)
        {

        }

        public void Press(object parameter)
        {
            if (Children.Count > 0)
                return;
            if (PressCommand.CanExecute(parameter))
                PressCommand.Execute(parameter);
        }
        
        protected override MenuItem Create(string id, short ordinalIndex, string group)
        {
            MenuButton clone = new MenuButton(id, ordinalIndex, AllowIdDuplicates);
            clone.PressCommand = PressCommand;
            return clone;
        }
    }
}