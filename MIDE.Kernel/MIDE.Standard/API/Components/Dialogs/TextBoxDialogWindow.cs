namespace MIDE.API.Components
{
    public class TextBoxDialogWindow : BaseDialogWindow<string>
    {
        private TextBox textBox;

        public TextBoxDialogWindow(string title, string message) : base(DialogResult.Accept | DialogResult.Cancel, title)
        {
            StackPanel stack = new StackPanel("stack");
            stack.Orientation = StackOrientation.Horizontal;
            stack.AddChild(new Label("message", message));
            textBox = new TextBox("value");
            textBox.Margin = new Measurements.BoundingBox(10, 0, 0, 0);
            textBox.MinWidth = 150;
            textBox.MaxWidth = 300;
            stack.AddChild(textBox);

            Body = stack;
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
    }
}