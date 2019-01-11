using System;

namespace MIDE.API.Components
{
    public class CheckBox : LayoutComponent
    {
        private bool isChecked;
        private string caption;

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
        public string Caption
        {
            get => caption;
            set
            {
                if (value == caption)
                    return;
                caption = value;
                OnPropertyChanged(nameof(Caption));
            }
        }

        public event Action<bool> CheckedChanged;

        public CheckBox(string id) : base(id) {}

        protected virtual void OnCheckedChanged() {}
    }
}