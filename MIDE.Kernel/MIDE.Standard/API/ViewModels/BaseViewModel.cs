using System;
using System.ComponentModel;

namespace MIDE.Standard.API.ViewModels
{
    [Serializable]
    public class BaseViewModel : INotifyPropertyChanged
    {
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}