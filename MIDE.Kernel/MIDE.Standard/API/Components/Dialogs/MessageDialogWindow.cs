namespace MIDE.API.Components
{
    public class MessageDialogWindow : BaseDialogWindow<bool>
    {
        public MessageDialogWindow(string title, string message) : base(DialogResult.Ok, title)
        {
            Body = new Panel("body");
            Body.AddChild(new Label("message", message));
        }

        public override bool GetData() => true;

        protected override void Validate() { }
    }
}