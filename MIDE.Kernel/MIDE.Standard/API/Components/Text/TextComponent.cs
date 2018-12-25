namespace MIDE.API.Components
{
    public abstract class TextComponent : LayoutComponent
    {
        protected string text;

        public virtual string Text
        {
            get => text;
            set
            {
                text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        public TextComponent(string id) : base(id) { }
    }
}