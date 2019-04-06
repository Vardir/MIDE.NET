using System.Collections.Generic;

namespace MIDE.API.Components
{
    /// <summary>
    /// Displays text information for user
    /// </summary>
    public sealed class MessageDialogBox : BaseDialogBox<bool>
    {
        private DialogResult[] results           = new[] { DialogResult.Ok };
        private DialogResult[] validationIgnored = new[] { DialogResult.Ok };

        public string Message { get; }
        public override IEnumerable<DialogResult> Results => results;

        public MessageDialogBox(string title, string message) : base(title)
        {
            Message = message;
        }

        public override bool GetData() => true;

        protected override bool Validate() => true;
        protected override IEnumerable<DialogResult> GetValidationIgnoredResults() => validationIgnored;
    }
}