using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;

namespace Vardirsoft.XApp.API.Validations
{
    public abstract class Validation : INotifyDataErrorInfo
    {
        public abstract bool HasErrors { get; }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public abstract IEnumerable GetErrors(string propertyName);

        protected void InvokeErrorsChanged(DataErrorsChangedEventArgs eventArgs) => ErrorsChanged?.Invoke(this, eventArgs);
    }

    public abstract class Validation<T> : Validation, INotifyPropertyChanged
    {
        private readonly List<ValidationError> _validationMessages;

        public override bool HasErrors => _validationMessages.Count == 0;

        public event PropertyChangedEventHandler PropertyChanged;

        public Validation()
        {
            _validationMessages = new List<ValidationError>();
        }

        public abstract void Validate(T value);

        /// <summary>
        /// Receive all the validation errors for the current validation instance.
        /// </summary>
        public override IEnumerable GetErrors(string _) => _validationMessages;
        public IEnumerable<ValidationError> GetAllErrors() => _validationMessages;

        protected void ClearErrors()
        {
            _validationMessages.Clear();
            
            OnPropertyChanged(nameof(HasErrors));
        }
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        protected void AddError(string message, object value)
        {
            _validationMessages.Add(new ValidationError("value", message, value));
            
            OnPropertyChanged(nameof(HasErrors));
        }
    }
}