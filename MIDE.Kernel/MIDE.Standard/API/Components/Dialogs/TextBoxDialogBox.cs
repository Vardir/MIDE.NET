using System.Collections.Generic;

namespace MIDE.API.Components
{
    /// <summary>
    /// Basic dialog box with text input and simple validation if user didn't entered anything
    /// </summary>
    public sealed class TextBoxDialogBox : BaseDialogBox<string>
    {
        private string message;
        private TextBox textBox;
        private DialogResult[] results           = new[] { DialogResult.Accept, DialogResult.Cancel };
        private DialogResult[] validationIgnored = new[] { DialogResult.Cancel };

        public override IEnumerable<DialogResult> Results => results;

        public TextBoxDialogBox(string title, string message) : base(title)
        {
            this.message = message;
            InitializeComponents();
        }

        public override string GetData() => textBox.Text;

        protected override void InitializeComponents()
        {
            body.ColumnMargin = 10;
            body.Columns.Add(new GridColumn("auto"));
            body.Columns.Add(new GridColumn("*"));
            body.Rows.Add(new GridRow("auto"));

            body.AddChild(new Label("message", message));
            textBox = new TextBox("value");
            textBox.MinWidth = 150;
            textBox.MaxWidth = 300;
            body.AddChild(textBox, 0, 2);
        }

        protected override bool Validate()
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
                return false;
            return true;
        }
        protected override GridLayout GenerateButtonsGrid(string id, IEnumerable<DialogButton> buttons)
        {
            return GetGridButtonsCentered(id, buttons);
        }
        protected override IEnumerable<DialogResult> GetValidationIgnoredResults() => validationIgnored;
    }
}