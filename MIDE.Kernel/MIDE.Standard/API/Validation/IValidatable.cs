using System.ComponentModel;
using System.Collections.ObjectModel;

namespace MIDE.API.Validations
{
    public interface IValidate : INotifyPropertyChanged
    {
        ObservableCollection<Validation> Validations { get; }
    }
}