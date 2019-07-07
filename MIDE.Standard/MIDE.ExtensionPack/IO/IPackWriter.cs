namespace MIDE.ExtensionPack
{
    public interface IPackWriter
    {
        void WritePack(IExtensionPack pack, string path);
    }
}