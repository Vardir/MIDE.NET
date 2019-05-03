namespace MIDE.API.Components
{
    public class ContextMenu : MenuItem
    {
        public ContextMenu(string id) : base(id, 0) { }

        protected override MenuItem Create(string id, short _, string __)
        {
            ContextMenu clone = new ContextMenu(id);
            return clone;
        }
    }
}