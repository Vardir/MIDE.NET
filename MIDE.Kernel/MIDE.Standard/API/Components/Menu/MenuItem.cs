using System;

namespace MIDE.Standard.API.Components
{
    public abstract class MenuItem : Button
    {
        public Menu ParentMenu { get; internal set; }
        public MenuItem ParentItem { get; internal set; }

        public override event Action Pressed;

        public abstract MenuItem this[string id] { get; }

        public MenuItem(string id) : base(id) { }

        public override void Press() { }

        public abstract void Add(MenuItem item, string parentId);
        public abstract void AddAfter(MenuItem item, string id);
        public abstract void AddBefore(MenuItem item, string id);

        public abstract MenuItem Find(string id);
    }
}