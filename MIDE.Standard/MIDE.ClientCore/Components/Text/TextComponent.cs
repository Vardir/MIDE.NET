namespace MIDE.Components
{
    public abstract class TextComponent : LayoutComponent
    {
        protected string text;

        public virtual string Text
        {
            get => text;
            set => SetAndNotify(value, ref text);
        }

        public TextComponent(string id) : base(id)
        {
            
        }
    }
}