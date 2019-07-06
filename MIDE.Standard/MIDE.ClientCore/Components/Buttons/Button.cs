using MIDE.API;
using MIDE.Visuals;

namespace MIDE.Components
{

    public class Button : LayoutComponent, IButton
    {
        private string caption;
        private Glyph glyph;

        public string Caption
        {
            get => caption;
            set
            {
                string localized = localization[value];
                if (localized == caption)
                    return;
                caption = localized;
                OnPropertyChanged(nameof(Caption));
            }
        }
        public Glyph ButtonGlyph
        {
            get => glyph;
            set
            {
                if (value == glyph)
                    return;
                glyph = value;
                OnPropertyChanged(nameof(ButtonGlyph));
            }
        }
        public ICommand PressCommand { get; set; }
        
        public Button(string id) : base(id)
        {
            Caption = $"({id})";
        }

        public virtual void Press(object parameter)
        {
            if (PressCommand.CanExecute(parameter))
                PressCommand.Execute(parameter);
        }

        protected override LayoutComponent CloneInternal(string id)
        {
            Button clone = Create(id);
            clone.caption = caption;
            clone.glyph = glyph;
            clone.PressCommand = PressCommand;
            return clone;
        }
        protected virtual Button Create(string id) => new Button(id);
    }
}