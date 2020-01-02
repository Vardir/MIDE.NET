using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

using Vardirsoft.XApp.API.Validations;

namespace Vardirsoft.XApp.Components
{
    public class TextBox : TextComponent
    {
        private bool _hasErrors;
        private bool _isReadonly;

        protected readonly ObservableCollection<ValidationError> validationErrors;

        public bool HasErrors
        {
            [DebuggerStepThrough]
            get => _hasErrors;
            
            [DebuggerStepThrough]
            private set => SetWithNotify(ref _hasErrors, value);
        }
        
        public bool IsReadonly
        {
            [DebuggerStepThrough]
            get => _isReadonly;
            
            [DebuggerStepThrough]
            set => SetWithNotify(ref _isReadonly, value);
        }
        
        public override string Text
        {
            [DebuggerStepThrough]
            get => text ?? Default;
            set
            {
                if (text == value || IsReadonly || !IsEnabled)
                    return;

                HasErrors = false;
                validationErrors.Clear();
                Validations.ForEach(v => v.Validate(value, nameof(Text), ValidationErrors));

                if (ValidationErrors.Count == 0)
                {
                    text = value;
                    NotifyPropertyChanged();
                }
                else
                {
                    HasErrors = true;
                }
            }
        }
        
        public string Default { get; }
        
        public List<ValueValidation<string>> Validations { get; }
        
        public ReadOnlyObservableCollection<ValidationError> ValidationErrors { get; }

        public TextBox(string id, string defaultValue = null) : base(id)
        {
            Default = defaultValue;
            Validations = new List<ValueValidation<string>>();
            validationErrors = new ObservableCollection<ValidationError>();
            ValidationErrors = new ReadOnlyObservableCollection<ValidationError>(validationErrors);
        }

        public void Clear()
        {
            if (IsEnabled)
                Text = Default;
        }

        protected override LayoutComponent CloneInternal(string id)
        {
            var clone = new TextBox(id, Default);
            clone._isReadonly = _isReadonly;
            clone.text = text;

            return clone;
        }

        [DebuggerStepThrough]
        protected virtual TextBox Create(string id) => new TextBox(id, Default);
    }
}