using System;

namespace MIDE.Standard.API.Components
{
    public class Button : LayoutComponent
    {
        private string caption;

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
        public event Action Pressed;
        
        public Button(string id) : base(id) { }

        public void Press() => Pressed?.Invoke();
    }
}