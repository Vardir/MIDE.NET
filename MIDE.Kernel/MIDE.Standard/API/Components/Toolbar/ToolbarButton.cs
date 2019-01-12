namespace MIDE.API.Components
{
    public class ToolbarButton : Button, IToolBarItem
    {
        public int Order { get; set; }
        public string Group { get; set; }

        public ToolbarButton(string id) : base(id) {}
    }
}