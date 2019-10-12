using System.Collections.Generic;

namespace Vardirsoft.XApp.Components
{
    /// <summary>
    /// Asks user a question and returns Yes or No
    /// </summary>
    public sealed class QuestionDialogBox : BaseDialogBox<bool>
    {
        private DialogResult[] validationIgnored = new[] { DialogResult.Yes, DialogResult.No };

        public string Question { get; }
        public DialogButton YesButton { get; }
        public DialogButton NoButton { get; }

        public QuestionDialogBox(string title, string question) : base(title)
        {
            Question  = question;
            YesButton = new DialogButton(this, DialogResult.Yes);
            NoButton  = new DialogButton(this, DialogResult.No);
        }

        public override bool GetData() => true;

        protected override bool Validate() => true;
        protected override IEnumerable<DialogResult> GetValidationIgnoredResults() => validationIgnored;
    }
}