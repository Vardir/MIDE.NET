namespace XApp.Components
{
    public class ToolbarActionTextBox : ActionTextBox, IToolBarItem
    {
        public int Order { get; set; }
        public string Group { get; set; }

        public ToolbarActionTextBox(string id, string defaultValue = null) : base(id, defaultValue) { }

        protected override TextBox Create(string id)
        {
            var clone = new ToolbarActionTextBox(id, Default);
            clone.Order = Order;
            clone.Group = Group;
            
            return clone;
        }
    }
}