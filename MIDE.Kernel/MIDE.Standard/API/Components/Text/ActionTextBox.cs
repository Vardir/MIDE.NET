namespace MIDE.API.Components
{
    public class ActionTextBox : TextBox
    {
        private Button actionButton;

        public Button ActionButton
        {
            get => actionButton;
            private set
            {
                if (value == actionButton || value == null)
                    return;
                actionButton = value;
                OnPropertyChanged(nameof(ActionButton));
            }
        }

        public ActionTextBox(string id, string defaultValue = null) : base(id, defaultValue)
        {
            ActionButton = new Button("action");
        }
    }
}