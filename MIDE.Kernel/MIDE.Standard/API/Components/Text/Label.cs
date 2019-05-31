namespace MIDE.API.Components
{
    public class Label : TextComponent
    {
        public Label(string id, string text) : base(id)
        {
            Text = localization[text];
        }

        protected override LayoutComponent CloneInternal(string id)
        {
            Label clone = new Label(id, text);
            return clone;
        }
    }
}