namespace MIDE.Standard.API.Components
{
    public class MenuConstructionContext
    {
        private readonly Menu menu;

        public MenuConstructionContext(Menu menu)
        {
            this.menu = menu;
        }

        public void AddItem(MenuItem item) => menu.AddChild(item);
        public void AddItem(string path, MenuItem item) => menu.AddChild(path, item);
        public void AddAfter(MenuItem item, string id) => menu.AddAfter(item, id);
        public void AddAfter(MenuItem item, string id, string parentId) => menu.AddAfter(item, id, parentId);
        public void AddBefore(MenuItem item, string id) => menu.AddBefore(item, id);
        public void AddBefore(MenuItem item, string id, string parentId) => menu.AddBefore(item, id, parentId);

        public bool Contains(string id) => menu.Contains(id);
    }
}