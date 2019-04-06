using System.Linq;
using System.Collections.Generic;

namespace MIDE.API.Components
{
    /// <summary>
    /// Basic dialog box with text input and simple validation if user didn't entered anything
    /// </summary>
    public sealed class TextBoxDialogBox : BaseDialogBox<string>
    {
        private DialogResult[] results           = new[] { DialogResult.Accept, DialogResult.Cancel };
        private DialogResult[] validationIgnored = new[] { DialogResult.Cancel };

        public string Message { get; }
        public TextBox Input { get; }
        public override IEnumerable<DialogResult> Results => results;

        public TextBoxDialogBox(string title, string message, string defaultValue = null) : base(title)
        {
            Message = message;
        }

        public override string GetData() => Message;

        protected override bool Validate() => Input.Validations.All(v => !v.HasErrors);
        protected override IEnumerable<DialogResult> GetValidationIgnoredResults() => validationIgnored;
    }
}