using System;

namespace MIDE.Standard.API.Commands
{
    public class RelayParameterizedCommand : ICommand
    {
        private readonly Action<object> action;

        public event EventHandler CanExecuteChanged = (sender, e) => { };

        public RelayParameterizedCommand(Action<object> action)
        {
            this.action = action;
        }

        public void Execute(object parameter) => action?.Invoke(parameter);
        
        public bool CanExecute(object parameter) => true;
    }
}
