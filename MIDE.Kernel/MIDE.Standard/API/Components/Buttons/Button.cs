using System;
using MIDE.Standard.Helpers;

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
        public virtual event Action Pressed;
        
        public Button(string id) : base(id)
        {
            Caption = this.FormatId();
        }

        public virtual void Press() => Pressed?.Invoke();
    }
}