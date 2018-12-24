using MIDE.Standard.API.Commands;

namespace MIDE.Standard.API.Components
{
    public class Button : LayoutComponent
    {
        private string caption;

        public string Caption
        {
            get => caption;
            set
            {
                if (value == caption)
                    return;
                caption = value;
                OnPropertyChanged(nameof(Caption));
            }
        }
        public ICommand PressCommand { get; set; }
        
        public Button(string id) : base(id)
        {
            Caption = FormatId();
        }

        public virtual void Press(object parameter)
        {
            if (PressCommand.CanExecute(parameter))
                PressCommand.Execute(parameter);
        }
    }
}