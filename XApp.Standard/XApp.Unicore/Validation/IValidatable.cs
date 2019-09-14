using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace XApp.API.Validations
{
    public interface IValidate : INotifyPropertyChanged
    {
        bool HasErrors { get; }
        IEnumerable<string> Errors { get; }
        ObservableCollection<Validation> Validations { get; }

        void NotifyError();
    }
}