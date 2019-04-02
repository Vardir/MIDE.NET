namespace MIDE.API.Components
{
    public class QuestionDialogWindow : BaseDialogWindow<bool>
    {
        public QuestionDialogWindow(string title, string question) : base(DialogResult.Yes | DialogResult.No, title)
        {
            Body = new Panel("body");
            Body.AddChild(new Label("question", question));
        }

        public override bool GetData() => true;

        protected override void Validate() {}
    }
}