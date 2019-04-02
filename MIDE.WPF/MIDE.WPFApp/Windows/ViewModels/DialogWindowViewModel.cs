using System.Windows;
using MIDE.API.Components;
using System.Collections.ObjectModel;

namespace MIDE.WPFApp
{
    public class DialogWindowViewModel : BaseWindowViewModel
    {
        private LayoutContainer body;
        private Button[] buttons;
        private ObservableCollection<string> validationErrors;

        public LayoutContainer Body
        {
            get => body;
            set
            {
                if (value == body)
                    return;
                body = value;
                OnPropertyChanged(nameof(Body));
            }
        }
        public Button[] Buttons
        {
            get => buttons;
            set
            {
                if (value == buttons)
                    return;
                buttons = value;
                OnPropertyChanged(nameof(Buttons));
            }
        }
        public ObservableCollection<string> ValidationErrors
        {
            get => validationErrors;
            set
            {
                if (value == validationErrors)
                    return;
                validationErrors = value;
                OnPropertyChanged(nameof(ValidationErrors));
            }
        }

        public DialogWindowViewModel(Window window) : base(window)
        {
            WindowMinimumWidth = 290;
            WindowMinimumHeight = 180;
            InnerContentPadding = new Thickness(2);
        }
    }
}