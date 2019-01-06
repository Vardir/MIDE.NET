using MIDE.Helpers;

namespace MIDE.API.Components
{
    public abstract class MenuItem : Button
    {
        /// <summary>
        /// Minimum ordinal index for menu item, specify for the item that always should be the first one
        /// </summary>
        public const short MIN_ORDINAL = -99;
        /// <summary>
        /// Maximum ordinal index for menu item, specify for the item that always should be the last one
        /// </summary>
        public const short MAX_ORDINAL = 99;

        /// <summary>
        /// Ordinal index identifying item positioning in list
        /// </summary>
        public short OrdinalIndex { get; }
        public Menu ParentMenu { get; internal set; }
        public MenuItem ParentItem { get; internal set; }

        public abstract MenuItem this[string id] { get; }

        public MenuItem(string id, short ordinalIndex) : base(id)
        {
            OrdinalIndex = ordinalIndex.Clamp(MIN_ORDINAL, MAX_ORDINAL);
        }

        public override void Press(object _) { }

        /// <summary>
        /// Add the given item to child items. If the parent ID given, searches (recursively if the flag specified) an item with the specified ID
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <param name="parentId">The parent to add this item to</param>
        /// <param name="searchRecursively">Search parent recursively</param>
        public abstract void Add(MenuItem item, string parentId, bool searchRecursively = false);

        public abstract MenuItem Find(string id, bool searchRecursively = false);
        /// <summary>
        /// Searches for the last item in the given path and produces it's child items ordinal indexes
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract (string, short)[] GetItemsOrdinals();
        /// <summary>
        /// Searches for the last item in the given path and produces out array of all child items ID
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract string[] GetAllItemsIDs();
    }
}