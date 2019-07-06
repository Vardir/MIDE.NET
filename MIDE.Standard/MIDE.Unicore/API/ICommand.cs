using System;

namespace MIDE.API
{
    public interface ICommand
    {
        event EventHandler CanExecuteChanged;

        bool CanExecute(object parameter);
        void Execute(object parameter);
    }
}