using System.Collections.Generic;

namespace XApp.Components
{
    /// <summary>
    /// Displays text information for user
    /// </summary>
    public sealed class MessageDialogBox : BaseDialogBox<bool>
    {
        private DialogResult[] validationIgnored = new[] { DialogResult.Ok };

        public string Message { get; }
        public DialogButton OkButton { get; }

        public MessageDialogBox(string title, string message) : base(title)
        {
            Message = localization[message];
            OkButton = new DialogButton(this, DialogResult.Ok);
        }

        public override bool GetData() => true;
        
        protected override bool Validate() => true;
        protected override IEnumerable<DialogResult> GetValidationIgnoredResults() => validationIgnored;
    }
}