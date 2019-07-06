using MIDE.API;

namespace MIDE.Components
{
    public interface IButton
    {
        ICommand PressCommand { get; set; }

        void Press(object parameter);
    }
}