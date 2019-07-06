using System;
using MIDE.API;

namespace MIDE.Commands
{
    public class RelayCommand : ICommand
    {
        protected readonly Action action;

        public event EventHandler CanExecuteChanged = (sender, e) => { };

        public RelayCommand(Action action)
        {
            this.action = action;
        }

        public void Execute(object parameter) => action?.Invoke();

        public bool CanExecute(object parameter) => true;
        public T Cast<T>()
            where T: RelayCommand
        {
            return (T)Activator.CreateInstance(typeof(T), action);
        }
    }
}