using MIDE.API.Validations;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MIDE.API.Components
{
    public class TextBox : TextComponent
    {
        private bool isReadonly;

        public bool HasErrors { get; private set; }
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
                HasErrors = false;
                ValidationErrors.Clear();
                Validations.ForEach(v => v.Validate(nameof(Text), value, ValidationErrors));
                if (ValidationErrors.Count != 0)
                {
                    HasErrors = true;
                    return;
                }
                text = value;
                OnPropertyChanged(nameof(Text));
            }
        }
        public string Default { get; }
        public List<ValueValidation<string>> Validations { get; }
        public ObservableCollection<ValidationError> ValidationErrors { get; }

        public TextBox(string id, string defaultValue = null) : base(id)
        {
            Default = defaultValue;
            Validations = new List<ValueValidation<string>>();
            ValidationErrors = new ObservableCollection<ValidationError>();
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