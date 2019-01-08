namespace MIDE.API.Components
{
    public class TextBox : TextComponent
    {
        private bool isReadonly;

        public bool IsReadonly
        {
            get => isReadonly;
            set
            {
                if (value == isReadonly)
                    return;
                isReadonly = value;
                OnPropertyChanged(nameof(IsReadonly));
            }
        }
        public override string Text
        {
            get => text ?? Default;
            set
            {
                if (text == value || IsReadonly || !IsEnabled)
                    return;
                text = value;
                OnPropertyChanged(nameof(Text));
            }
        }
        public string Default { get; }

        public TextBox(string id, string defaultValue = null) : base(id)
        {
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