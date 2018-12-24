using System;

namespace MIDE.Standard.API.Commands
{
    public interface ICommand
    {
        event EventHandler CanExecuteChanged;

        bool CanExecute(object parameter);
        void Execute(object parameter);
    }
}