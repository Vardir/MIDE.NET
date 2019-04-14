using MIDE.Helpers;

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

        protected override Button Create(string id)
        {
            MenuButton clone = new MenuButton(id, OrdinalIndex, AllowIdDuplicates);
            clone.Children.AddRange(Children.Select(item => item.Clone() as MenuItem));
            return clone;
        }
        protected override MenuItem Create(string id, short ordinalIndex)
        {
            MenuButton clone = new MenuButton(id, ordinalIndex, AllowIdDuplicates);
            clone.Children.AddRange(Children.Select(item => item.Clone() as MenuItem));
            return clone;
        }
    }
}