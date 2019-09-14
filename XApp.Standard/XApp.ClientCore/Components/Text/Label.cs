namespace XApp.Components
{
    public class Label : TextComponent
    {
        public Label(string id, string text) : base(id)
        {
            Text = localization[text];
        }

        protected override LayoutComponent CloneInternal(string id)
        {
            var clone = new Label(id, text);
            
            return clone;
        }
    }
}