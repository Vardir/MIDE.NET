using System;
using MIDE.Standard.API.Commands;

namespace MIDE.WPFApp.RelayCommands
{
    public class WindowsRelayCommand : RelayCommand, System.Windows.Input.ICommand
    {
        public WindowsRelayCommand(Action action) : base(action) {}
    }
}