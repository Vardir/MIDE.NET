using MIDE.Standard.API.Validation;

namespace MIDE.Standard.API.Components
{
    public class TextBox : TextComponent
    {
        private bool isValid;

        public bool IsValid
        {
            get => IsValid;
            private set
            {
                if (isValid == value)
                    return;
                isValid = value;
                OnPropertyChanged(nameof(IsValid));
            }
        }
        public override string Text
        {
            get => text;
            set
            {
                if (text == value || !IsEnabled)
                    return;
                text = value;
                IsValid = Validator.IsValid(text);
                if (isValid)
                    LastValid = value;
                OnPropertyChanged(nameof(Text));
            }
        }
        public string Default { get; }
        public string LastValid { get; private set; }
        public IValidator<string> Validator { get; }

        public TextBox(string id, string defaultValue = null, IValidator<string> validator = null) : base(id)
        {
            Validator = validator;
            Default = defaultValue;
        }

        public void Clear()
        {
            if (!IsEnabled)
                return;
            Text = Default;
        }
    }
}