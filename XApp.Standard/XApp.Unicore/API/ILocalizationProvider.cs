namespace Vardirsoft.XApp.API
{
    public interface ILocalizationProvider
    {
        string this[string str] { get; }

        void LoadFrom(string file);
    }
}