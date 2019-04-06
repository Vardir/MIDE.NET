using MIDE.API.Measurements;
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

        public override IEnumerable<DialogResult> Results => results;

        public MessageDialogBox(string title, string message) : base(title)
        {
            body.Rows.Add(new GridRow("*"));
            body.AddChild(new Label("message", message) { HorizontalAlignment = HorizontalAlignment.Center });
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