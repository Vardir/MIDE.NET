namespace MIDE.API.Components
{
    public class MenuButton : MenuGroup
    {
        public MenuButton(string id, short ordinalIndex, bool allowDuplicates = false) : base(id, ordinalIndex, allowDuplicates) { }

        public override void Press(object parameter)
        {
            if (Children.Count > 0)
                return;
            if (PressCommand.CanExecute(parameter))
                PressCommand.Execute(parameter);
        }
    }
}