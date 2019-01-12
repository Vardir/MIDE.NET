namespace MIDE.API.Components
{
    public class ToolbarActionTextBox : ActionTextBox, IToolBarItem
    {
        public int Order { get; }
        public string Group { get; }

        public ToolbarActionTextBox(string id, string defaultValue = null) : base(id, defaultValue) { }
    }
}