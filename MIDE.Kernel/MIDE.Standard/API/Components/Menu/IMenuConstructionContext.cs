namespace MIDE.Standard.API.Components
{
    /// <summary>
    /// Provides the simple construction interface to menu that allows only possibility to add items and receive basic information
    /// </summary>
    public interface IMenuConstructionContext
    {
        void AddItem(MenuItem item);
        void AddItem(string path, MenuItem item);

        bool Contains(string id);
        (string, short)[] GetItemsOrdinals(string path);
        string[] GetAllPaths(string path);
    }
}