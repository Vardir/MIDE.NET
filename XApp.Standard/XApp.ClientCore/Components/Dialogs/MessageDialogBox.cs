using System.Collections.Generic;

namespace Vardirsoft.XApp.Components
{
    /// <summary>
    /// Displays text information for user
    /// </summary>
    public sealed class MessageDialogBox : BaseDialogBox<bool>
    {
        private readonly DialogResult[] validationIgnored = { DialogResult.Ok };

        public string Message { get; }
        
        public DialogButton OkButton { get; }

        public MessageDialogBox(string title, string message, DialogMode mode = DialogMode.Normal) : base(title, mode)
        {
            Message = Localization[message];
            OkButton = new DialogButton(this, DialogResult.Ok);
        }

        public override bool GetData() => true;
        
        protected override bool Validate() => true;
        protected override IEnumerable<DialogResult> GetValidationIgnoredResults() => validationIgnored;
    }
}