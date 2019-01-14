using System;
using System.Collections;
using System.ComponentModel;

namespace MIDE.API.Validation
{
    public abstract class Validator : INotifyDataErrorInfo
    {
        public abstract bool HasErrors { get; }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public abstract IEnumerable GetErrors(string propertyName);

        protected void InvokeErrorsChanged(DataErrorsChangedEventArgs eventArgs)
        {
            ErrorsChanged?.Invoke(this, eventArgs);
        }
    }
}