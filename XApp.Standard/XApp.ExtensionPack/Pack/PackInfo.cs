namespace Vardirsoft.XApp.ExtensionPack
{
    /// <summary>
    /// A set of basic description info for extension pack
    /// </summary>
    public struct PackInfo
    {
        public readonly string Id;
        public readonly string Title;
        public readonly string Description;
        public readonly string ReleaseNotes;

        public PackInfo(string id, string title, string description, string releaseNotes)
        {
            Id = id;
            Title = title;
            Description = description;
            ReleaseNotes = releaseNotes;
        }
    }
}