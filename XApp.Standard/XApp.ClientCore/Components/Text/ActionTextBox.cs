using Vardirsoft.Shared.MVVM;

namespace Vardirsoft.XApp.Components
{
    public class ActionTextBox : TextBox
    {
        private BaseCommand actionCommand;

        public BaseCommand ActionCommand
        {
            get => actionCommand;
            set => SetWithNotify(ref actionCommand, value, true);
        }

        public ActionTextBox(string id, string defaultValue = null) : base(id, defaultValue)
        {
            
        }

        protected override TextBox Create(string id)
        {
            var clone = new ActionTextBox(id, Default);
            clone.actionCommand = actionCommand;
            
            return clone;
        }
    }
}