using System;
using System.Collections.ObjectModel;

namespace Vardirsoft.XApp.ExtensionPack
{
    public class XPack : IExtensionPack
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Copyright { get; set; }
        public string Description { get; set; }
        public string ReleaseNotes { get; set; }
        public Version Version { get; set; }
        public byte[] IconBytes { get; set; }
        public string[] Tags { get; set; }
        public string[] Owners { get; set; }
        public string[] Authors { get; set; }
        public ReadOnlyCollection<Dependency> AppDependencies { get; set; }
        public ReadOnlyCollection<Dependency> PackDependencies { get; set; }
    }
}