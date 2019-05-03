using MIDE.API.Commands;

namespace MIDE.API.Components
{
    public interface IButton
    {
        ICommand PressCommand { get; set; }

        void Press(object parameter);
    }
}