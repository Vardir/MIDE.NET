using System;
using MIDE.API.Commands;

namespace MIDE.WPFApp.RelayCommands
{
    public class WindowsRelayParameterizedCommand : RelayParameterizedCommand, System.Windows.Input.ICommand
    {
        public WindowsRelayParameterizedCommand(Action<object> action) : base (action) {}
    }
}