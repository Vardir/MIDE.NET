using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Newtonsoft.Json;

using Vardirsoft.Shared.Helpers;

using Vardirsoft.XApp.API;
using Vardirsoft.XApp.IoC;
using Vardirsoft.XApp.Logging;
using Vardirsoft.XApp.FileSystem;
using Vardirsoft.XApp.Extensibility;
using Vardirsoft.XApp.Schemes.JSON;

namespace Vardirsoft.XApp.Application
{
    /// <summary>
    /// Application extension manager that helps installing, enabling/disabling and uninstalling extensions
    /// </summary>
    public sealed class ExtensionsManager : IDisposable
    {
        private readonly Dictionary<string, AppExtensionEntry> _appExtensions;

        public ExtensionsManager()
        {
            _appExtensions = new Dictionary<string, AppExtensionEntry>();
        }
        
        public void LoadExtensions()
        {
            var fileManager = IoCContainer.Resolve<FileManager>();
            var root = ApplicationPaths.Instance[ApplicationPaths.EXTENSIONS];
            var configPath = fileManager.Combine(root, "appx.json");

            try
            {
                var fileData = IoCContainer.Resolve<FileManager>().TryRead(configPath);
                var pathItems = JsonConvert.DeserializeObject<ExtensionEntry[]>(fileData);
                
                pathItems.Select(LoadExtension).ForEach(RegisterExtension);
            }
            catch (Exception ex)
            {
                IoCContainer.Resolve<ILogger>().PushError(ex, this, "Couldn't load extensions: empty descriptions file or invalid syntax");
            }
            
            IoCContainer.Resolve<ILogger>().PushInfo("Extensions loaded");
        }
        
        public void Dispose()
        {
            // if (_disposed)
            //     return;
            //     
            // UnloadExtensions();
            // _serializerSettings = null;
            // _registered = null;
            // _disposed = true;
        }

        private void RegisterExtension(AppExtensionEntry entry)
        {
            if (_appExtensions.ContainsKey(entry.Id))
                throw new ArgumentException($"Duplicate extension entry: ID {entry.Id}");
            
            _appExtensions.Add(entry.Id, entry);
        }
        
        private AppExtensionEntry LoadExtension(ExtensionEntry entry)
        {
            var fileManager = IoCContainer.Resolve<IFileManager>();
            var path = entry.Path.HasValue() ? entry.Path : fileManager.Combine(ApplicationPaths.Instance[ApplicationPaths.EXTENSIONS], entry.Id, $"{entry.Id}.dll");
            
            return fileManager.OpenBinaryFile(path, reader =>
            {
                using var memoryStream = new MemoryStream();
                var metadataLength = reader.ReadInt32();
                var metadataBytes = reader.ReadBytes(metadataLength);
                
                memoryStream.Write(metadataBytes, 0, metadataLength);

                var extensionMetadata = AppMetadata.Load<ExtensionMetadata>(memoryStream);
                var assemblyBytes = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));                
                var assembly = Assembly.Load(assemblyBytes);
                var extensionType = assembly.GetExportedTypes().FirstOrDefault(x => x.IsSubclassOf(typeof(AppExtension)));
                
                if (extensionType is null)
                    throw new NullReferenceException();
                
                var extensionInstance = (AppExtension)Activator.CreateInstance(extensionType);
                
                return new AppExtensionEntry(extensionInstance) 
                {
                    Id = extensionMetadata.Id,
                    IsEnabled = entry.IsEnabled,
                    Copyright = extensionMetadata.Copyright,
                    Description = extensionMetadata.Description,
                    Origin = path,
                    Tags = extensionMetadata.Tags,
                    Title = extensionMetadata.Title,
                    Version = new Version(extensionMetadata.Version),
                    LiceseUrl = new Uri(extensionMetadata.LiceseUrl),
                    ProjectUrl = new Uri(extensionMetadata.ProjectUrl),
                    Owners = extensionMetadata.Owners.Split(',').Select(x => x.Trim()),
                    Authors = extensionMetadata.Authors.Split(',').Select(x => x.Trim())
                };
            });
        }
    }
    
    public class AppExtensionEntry
    {
        private HashSet<string> _dependants;
        private HashSet<string> _dependencies;

        #region Properties

        public string Id { get; internal set; }
        
        public bool IsEnabled { get; internal set; }
        
        public string Title { get; internal set; }
        
        public string Description { get; internal set; }
        
        public string Copyright { get; internal set; }   
             
        /// <summary>
        /// Original extension path from which it was loaded
        /// </summary>
        public string Origin { get; internal set; }
        
        public Uri LiceseUrl { get; internal set; }
        
        public Uri ProjectUrl { get; internal set; }
        
        public Version Version { get; internal set; }
        
        public string[] Tags { get; internal set; }
        
        public int DependantsCount { [DebuggerStepThrough] get => _dependants.Count; }
        
        public int DependenciesCount { [DebuggerStepThrough] get => _dependencies.Count; }
        
        public IEnumerable<string> Dependencies { [DebuggerStepThrough] get => _dependencies; }
        
        public IEnumerable<string> Dependants { [DebuggerStepThrough] get => _dependants; }
        
        public IEnumerable<string> Owners { get; internal set; }
        
        public IEnumerable<string> Authors { get; internal set; }
        
        public AppExtension Extension { get; }

        #endregion // Properties

        public AppExtensionEntry(AppExtension extension)
        {
            Extension = extension;
        }

        public void AddDependent(string id) => _dependants.Add(id);
    }
}