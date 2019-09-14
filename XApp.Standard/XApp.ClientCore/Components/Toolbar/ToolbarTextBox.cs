namespace XApp.Components
{
    public class ToolbarTextBox : TextBox, IToolBarItem
    {
        public int Order { get; set; }
        public string Group { get; set; }

        public ToolbarTextBox(string id, string defaultValue = null) : base(id, defaultValue) { }
    }
}