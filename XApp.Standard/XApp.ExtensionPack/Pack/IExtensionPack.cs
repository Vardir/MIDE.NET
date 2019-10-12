using System;
using System.Collections.ObjectModel;

namespace Vardirsoft.XApp.ExtensionPack
{
    /// <summary>
    /// An extension pack info
    /// </summary>
    public interface IExtensionPack
    {
        string Id { get; }
        string Title { get; }
        string Copyright { get; }
        string Description { get; }
        string ReleaseNotes { get; }
        Version Version { get; }
        byte[] IconBytes { get; }
        string[] Tags { get; }
        string[] Owners { get; }
        string[] Authors { get; }
        ReadOnlyCollection<Dependency> AppDependencies { get; }
        ReadOnlyCollection<Dependency> PackDependencies { get; }
    }
}