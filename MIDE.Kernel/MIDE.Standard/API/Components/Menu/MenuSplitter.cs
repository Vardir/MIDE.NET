using System;

namespace MIDE.Standard.API.Components
{
    public class MenuSplitter : MenuItem
    {
        public override MenuItem this[string id] => null;

        public MenuSplitter(string id) : base(id) { }

        public override void Add(MenuItem item, string parentId) => RaiseAddChildException();
        public override void AddAfter(MenuItem item, string id) => RaiseAddChildException();
        public override void AddBefore(MenuItem item, string id) => RaiseAddChildException();

        public override MenuItem Find(string id) => null;

        private void RaiseAddChildException() => throw new InvalidOperationException("Can not add child item into splitter");
    }
}