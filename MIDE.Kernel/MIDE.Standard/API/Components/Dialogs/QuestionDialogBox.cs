namespace MIDE.API.Components
{
    /// <summary>
    /// Asks user a question and returns Yes or No
    /// </summary>
    public class QuestionDialogBox : BaseDialogBox<bool>
    {
        public QuestionDialogBox(string title, string question) : base(title)
        {
            AddChild(new Label("question", question));

            SetDialogResults(DialogResult.Ok, DialogResult.No);
        }

        public override bool GetData() => true;

        protected override void Validate() {}
    }
}