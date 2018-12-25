using System;
using MIDE.API.Commands;

namespace MIDE.WPFApp.RelayCommands
{
    public class WindowsRelayCommand : RelayCommand, System.Windows.Input.ICommand
    {
        public WindowsRelayCommand(Action action) : base(action) {}
    }
}