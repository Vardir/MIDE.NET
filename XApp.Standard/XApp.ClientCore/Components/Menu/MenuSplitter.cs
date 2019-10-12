namespace Vardirsoft.XApp.Components
{
    public class MenuSplitter : MenuItem
    {
        public MenuSplitter(string id, int ordinalIndex) : base(id, ordinalIndex)
        {

        }
        public MenuSplitter(string id, string group, int ordinalIndex) : base(id, group, ordinalIndex)
        {

        }

        protected override MenuItem Create(string id, int ordinalIndex, string group)
        {
            MenuSplitter clone = new MenuSplitter(id, group, ordinalIndex);
            return clone;
        }
    }
}