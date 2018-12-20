namespace MIDE.Standard.API.Components
{
    /// <summary>
    /// Provides the simple construction interface to menu that allows only possibility to add items and receive basic information
    /// </summary>
    public interface IMenuConstructionContext
    {
        void AddItem(MenuItem item);
        void AddItem(string path, MenuItem item);
        void AddAfter(MenuItem item, string id);
        void AddAfter(MenuItem item, string id, string parentId);
        void AddBefore(MenuItem item, string id);
        void AddBefore(MenuItem item, string id, string parentId);

        bool Contains(string id);
    }
}