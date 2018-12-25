using System;

namespace MIDE.API.Components
{
    public class CheckBox : LayoutComponent
    {
        private bool isChecked;

        public bool IsChecked
        {
            get => isChecked;
            set
            {
                if (isChecked == value || !IsEnabled)
                    return;
                isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
                CheckedChanged?.Invoke(isChecked);
                OnCheckedChanged();
            }
        }

        public event Action<bool> CheckedChanged;

        public CheckBox(string id) : base(id) {}

        protected virtual void OnCheckedChanged() {}
    }
}