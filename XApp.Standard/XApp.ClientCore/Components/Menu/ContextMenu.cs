namespace Vardirsoft.XApp.Components
{
    public class ContextMenu : MenuItem
    {
        public ContextMenu(string id) : base(id, 0) { }

        protected override MenuItem Create(string id, int _, string __)
        {
            ContextMenu clone = new ContextMenu(id);
            
            return clone;
        }
    }
}