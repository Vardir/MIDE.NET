namespace MIDE.Standard.API.Components
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
                OnCheckedChanged();
            }
        }

        public CheckBox(string id) : base(id) {}

        protected virtual void OnCheckedChanged() {}
    }
}