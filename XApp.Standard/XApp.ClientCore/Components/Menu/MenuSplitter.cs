namespace XApp.Components
{
    public class MenuSplitter : MenuItem
    {
        public MenuSplitter(string id, short ordinalIndex) : base(id, ordinalIndex)
        {

        }
        public MenuSplitter(string id, string group, short ordinalIndex) : base(id, group, ordinalIndex)
        {

        }

        protected override MenuItem Create(string id, short ordinalIndex, string group)
        {
            MenuSplitter clone = new MenuSplitter(id, group, ordinalIndex);
            return clone;
        }
    }
}