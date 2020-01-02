using System;
using System.Diagnostics;

namespace Vardirsoft.XApp.Components
{
    public class CheckBox : LayoutComponent
    {
        private bool _isChecked;
        private string _caption;

        public bool IsChecked
        {
            [DebuggerStepThrough]
            get => _isChecked;
            set
            {
                if (_isChecked == value || !IsEnabled)
                    return;

                _isChecked = value;
                
                NotifyPropertyChanged();
                CheckedChanged?.Invoke(_isChecked);
                OnCheckedChanged();
            }
        }
        public string Caption
        {
            get => _caption;
            set => SetLocalizedAndNotify(value, ref _caption);
        }

        public event Action<bool> CheckedChanged;

        public CheckBox(string id) : base(id) {}

        protected override LayoutComponent CloneInternal(string id)
        {
            var clone = new CheckBox(id);
            clone._caption = _caption;

            return clone;
        }

        protected virtual void OnCheckedChanged() {}
    }
}