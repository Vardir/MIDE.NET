using MIDE.API.Validations;
using System.Collections.ObjectModel;

namespace MIDE.API.Components
{
    public class TextBox : TextComponent, IValidate
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
        public ObservableCollection<Validation> Validations { get; }

        public TextBox(string id, string defaultValue = null) : base(id)
        {
            Default = defaultValue;
            Validations = new ObservableCollection<Validation>();
        }

        public void Clear()
        {
            if (!IsEnabled)
                return;
            Text = Default;
        }

        protected override LayoutComponent CloneInternal(string id)
        {
            TextBox clone = new TextBox(id, Default);
            clone.isReadonly = isReadonly;
            clone.text = text;
            return clone;
        }

        protected virtual TextBox Create(string id) => new TextBox(id, Default);
    }
}