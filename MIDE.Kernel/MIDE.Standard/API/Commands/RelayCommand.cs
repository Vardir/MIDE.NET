using System;

namespace MIDE.Standard.API.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action action;

        public event EventHandler CanExecuteChanged = (sender, e) => { };

        public RelayCommand(Action action)
        {
            this.action = action;
        }

        public void Execute(object parameter) => action?.Invoke();

        public bool CanExecute(object parameter) => true;        
    }
}