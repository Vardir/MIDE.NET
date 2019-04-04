using MIDE.API.Measurements;
using System.Collections.Generic;

namespace MIDE.API.Components
{
    /// <summary>
    /// Basic dialog box with text input and simple validation if user didn't entered anything
    /// </summary>
    public class TextBoxDialogBox : BaseDialogBox<string>
    {
        private readonly TextBox textBox;

        public TextBoxDialogBox(string title, string message) : base(title)
        {
            Columns.Add(new GridColumn(new GridLength("auto")));
            Columns.Add(new GridColumn(new GridLength("10")));
            Columns.Add(new GridColumn(new GridLength("*")));
            Rows.Add(new GridRow(new GridLength("auto")));

            AddChild(new Label("message", message), 0, 0);
            textBox = new TextBox("value");
            textBox.MinWidth = 150;
            textBox.MaxWidth = 300;
            AddChild(textBox, 0, 2);

            SetDialogResults(DialogResult.Accept, DialogResult.Cancel);
        }

        public override string GetData() => textBox.Text;

        protected override void Validate()
        {
            ValidationErrors.Clear();
            if (SelectedResult == DialogResult.Cancel)
                return;
            if (string.IsNullOrWhiteSpace(textBox.Text))
                ValidationErrors.Add("Text is empty!");
        }
        protected override GridLayout GenerateGrid(string id, IEnumerable<DialogButton> buttons)
        {
            return GetGridButtonsCentered(id, buttons);
        }
    }
}