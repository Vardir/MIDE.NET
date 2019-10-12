namespace Vardirsoft.XApp.ExtensionPack
{
    /// <summary>
    /// Extension pack reader
    /// </summary>
    public interface IPackReader
    {
        /// <summary>
        /// Reads an extension pack from the given file or folder
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IExtensionPack Read(string path);
    }
}