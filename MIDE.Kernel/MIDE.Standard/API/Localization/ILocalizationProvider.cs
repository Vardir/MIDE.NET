namespace MIDE.API.Localization
{
    public interface ILocalizationProvider
    {
        string this[string key] { get; }

        (bool, string) TryGetString(string key);
    }
}