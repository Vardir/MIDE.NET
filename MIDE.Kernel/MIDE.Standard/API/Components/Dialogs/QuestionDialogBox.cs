using System.Collections.Generic;

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

            SetDialogResults(DialogResult.Yes, DialogResult.No);
        }

        public override bool GetData() => true;

        protected override void Validate() {}
        protected override GridLayout GenerateGrid(string id, IEnumerable<DialogButton> buttons)
        {
            return GetGridButtonsCentered(id, buttons);
        }
    }
}