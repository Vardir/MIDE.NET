using System.Linq;
using System.Collections.Generic;

namespace Vardirsoft.XApp.Components
{
    /// <summary>
    /// Basic dialog box with text input and simple validation if user didn't entered anything
    /// </summary>
    public sealed class TextBoxDialogBox : BaseDialogBox<string>
    {
        private DialogResult[] validationIgnored = new[] { DialogResult.Cancel };

        public string Message { get; }
        public TextBox Input { get; }
        public DialogButton AcceptButton { get; }
        public DialogButton CancelButton { get; }

        public TextBoxDialogBox(string title, string message, string defaultValue = null) : base(title, DialogMode.Modal)
        {
            Message = localization[message];
            Input = new TextBox("input", defaultValue);
            AcceptButton = new DialogButton(this, DialogResult.Accept);
            CancelButton = new DialogButton(this, DialogResult.Cancel);
        }

        public override string GetData() => Input.Text;

        protected override bool Validate() => Input.Validations.All(v => !v.HasErrors);
        protected override IEnumerable<DialogResult> GetValidationIgnoredResults() => validationIgnored;
    }
}