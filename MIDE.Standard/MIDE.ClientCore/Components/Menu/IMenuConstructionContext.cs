namespace MIDE.Components
{
    /// <summary>
    /// Provides a simple construction interface to menu that allows only possibility to add items and receive basic information about menu contents
    /// </summary>
    public interface IMenuConstructionContext
    {
        /// <summary>
        /// Adds an item into collection of the menu
        /// </summary>
        /// <param name="item"></param>
        void AddItem(MenuItem item);
        /// <summary>
        /// Adds an item by the given path. Specify '/' to place the item in menu root
        /// </summary>
        /// <param name="path"></param>
        /// <param name="item"></param>
        void AddItem(string path, MenuItem item);

        /// <summary>
        /// Verifies if item with the given ID is exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Contains(string id);
        /// <summary>
        /// Verifies if the given path is exist in the menu and it's sub-items
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool Exists(string path);
        /// <summary>
        /// Produces all the ID-ordinal value pairs for the specified path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        (string id, short ordinalIndex)[] GetItemsOrdinals(string path);
        /// <summary>
        /// Gets all the child items for the given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string[] GetAllPaths(string path);
    }
}