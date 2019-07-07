namespace MIDE.ExtensionPack
{
    /// <summary>
    /// Extension pack writer
    /// </summary>
    public interface IPackWriter
    {
        /// <summary>
        /// Writes the given extension pack to the given file or folder
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="path"></param>
        void WritePack(IExtensionPack pack, string path);
    }
}