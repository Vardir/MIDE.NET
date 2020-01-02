using System.Diagnostics;

using Vardirsoft.Shared.MVVM;

namespace Vardirsoft.XApp.Components
{
    public class ActionTextBox : TextBox
    {
        private BaseCommand _actionCommand;

        public BaseCommand ActionCommand
        {
            [DebuggerStepThrough]
            get => _actionCommand;
            
            [DebuggerStepThrough]
            set => SetWithNotify(ref _actionCommand, value, true);
        }

        public ActionTextBox(string id, string defaultValue = null) : base(id, defaultValue)
        {
            
        }

        protected override TextBox Create(string id)
        {
            var clone = new ActionTextBox(id, Default);
            clone._actionCommand = _actionCommand;
            
            return clone;
        }
    }
}