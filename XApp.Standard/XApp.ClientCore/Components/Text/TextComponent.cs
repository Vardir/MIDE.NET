namespace Vardirsoft.XApp.Components
{
    public abstract class TextComponent : LayoutComponent
    {
        protected string text;

        public virtual string Text
        {
            get => text;
            set => SetWithNotify(ref text, value, false);
        }

        public TextComponent(string id) : base(id)
        {
            
        }
    }
}