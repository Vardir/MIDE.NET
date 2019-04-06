using System.Collections.Generic;

namespace MIDE.API.Components
{
    /// <summary>
    /// Asks user a question and returns Yes or No
    /// </summary>
    public sealed class QuestionDialogBox : BaseDialogBox<bool>
    {
        private DialogResult[] results           = new[] { DialogResult.Yes, DialogResult.No };
        private DialogResult[] validationIgnored = new[] { DialogResult.Yes, DialogResult.No };

        public override IEnumerable<DialogResult> Results => results;

        public QuestionDialogBox(string title, string question) : base(title)
        {
            body.Rows.Add(new GridRow("*"));
            body.AddChild(new Label("question", question) { HorizontalAlignment = HorizontalAlignment.Center });
        }

        public override bool GetData() => true;

        protected override bool Validate() => true;
        protected override GridLayout GenerateButtonsGrid(string id, IEnumerable<DialogButton> buttons)
        {
            return GetGridButtonsCentered(id, buttons);
        }
        protected override IEnumerable<DialogResult> GetValidationIgnoredResults() => validationIgnored;
    }
}