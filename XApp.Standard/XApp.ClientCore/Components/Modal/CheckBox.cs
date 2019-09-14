using System;

namespace XApp.Components
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
                OnPropertyChanged();
                CheckedChanged?.Invoke(isChecked);
                OnCheckedChanged();
            }
        }
        public string Caption
        {
            get => caption;
            set => SetLocalizedAndNotify(value, ref caption);
        }

        public event Action<bool> CheckedChanged;

        public CheckBox(string id) : base(id) {}

        protected override LayoutComponent CloneInternal(string id)
        {
            var clone = new CheckBox(id);
            clone.caption = caption;

            return clone;
        }

        protected virtual void OnCheckedChanged() {}
    }
}