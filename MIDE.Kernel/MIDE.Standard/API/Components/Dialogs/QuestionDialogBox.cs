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

        public string Question { get; }
        public override IEnumerable<DialogResult> Results => results;

        public QuestionDialogBox(string title, string question) : base(title)
        {
            Question = question;
        }

        public override bool GetData() => true;

        protected override bool Validate() => true;
        protected override IEnumerable<DialogResult> GetValidationIgnoredResults() => validationIgnored;
    }
}