using System;

namespace MIDE.Standard.API.Components
{
    public class MenuSplitter : MenuItem
    {
        public override MenuItem this[string id] => null;

        public MenuSplitter(string id, short ordinalIndex) : base(id, ordinalIndex) { }

        public override void Add(MenuItem item, string parentId, bool recursively) => RaiseAddChildException();

        public override MenuItem Find(string id, bool recursively) => null;
        public override (string, short)[] GetItemsOrdinals() => null;
        public override string[] GetAllItemsIDs() => null;

        private void RaiseAddChildException() => throw new InvalidOperationException("Can not add child item into splitter");
    }
}