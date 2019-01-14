using System.ComponentModel;
using System.Collections.ObjectModel;

namespace MIDE.API.Validation
{
    public interface IValidate : INotifyPropertyChanged
    {
        ObservableCollection<Validator> Validators { get; }
    }
}