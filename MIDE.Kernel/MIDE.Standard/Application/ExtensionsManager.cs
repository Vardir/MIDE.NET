using NuGet;
using System;
using System.Linq;
using MIDE.Helpers;
using Newtonsoft.Json;
using MIDE.FileSystem;
using MIDE.Schemes.JSON;
using System.Reflection;
using MIDE.API.Components;
using MIDE.API.Extensibility;
using MIDE.Application.Logging;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;

namespace MIDE.Application
{
    /// <summary>
    /// Application extension manager that helps installing, enabling/disabling and uninstalling extensions
    /// </summary>
    public sealed class ExtensionsManager : ApplicationComponent, IDisposable
    {
        private static ExtensionsManager instance;
        public static ExtensionsManager Instance => instance ?? (instance = new ExtensionsManager());

        private bool _disposed;
        private Logger appLogger;
        private FileManager fileManager;
        private JsonSerializerSettings serializerSettings;
        private List<AppExtensionEntry> entries;
        private IPackageRepository localRespository;
        private DefaultPackagePathResolver localPathResolver;

        /// <summary>
        /// A set of registered application extensions
        /// </summary>
        public IEnumerable<AppExtension> Extensions => entries.Select(axe => axe.Extension);
        public IEnumerable<AppExtensionEntry> ExtensionsEntries => entries;

        private ExtensionsManager() : base("app-extension-manager")
        {
            appLogger = Kernel.AppLogger;
            fileManager = Kernel.FileManager;
            entries = new List<AppExtensionEntry>();
            serializerSettings = new JsonSerializerSettings();
            serializerSettings.Error = OnSerializationError;
        }

        /// <summary>
        /// Enables a previously installed extension by the given name, so it can be initialized in application.
        /// Changes applied after application restart.
        /// </summary>
        /// <param name="id"></param>
        public void Enable(string id)
        {
            var extension = entries.FirstOrDefault(ax => ax.Id == id);
            if (extension == null)
                return;
            extension.IsEnabled = true;
        }
        /// <summary>
        /// Disables a previously installed extension by the given name, so it will not be initialized in application.
        /// Changes applied after application restart.
        /// </summary>
        /// <param name="id"></param>
        public void Disable(string id)
        {
            var extension = entries.FirstOrDefault(ax => ax.Id == id);
            if (extension == null)
                return;
            extension.IsEnabled = true;
        }
        /// <summary>
        /// Loads all the extensions that are provided in attached assemblies
        /// </summary>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="ApplicationException"></exception>
        public void LoadExtensions()
        {
            appLogger.PushDebug(null, "Loading extensions");
            var directory = fileManager.GetAbsolutePath(fileManager[FileManager.EXTENSIONS]);
            localRespository = PackageRepositoryFactory.Default.CreateRepository(directory);
            localPathResolver = new DefaultPackagePathResolver(directory);
            foreach (var pack in localRespository.GetPackages())
            {
                LoadExtension(directory, pack);
            }
            appLogger.PushDebug(null, "Extensions loaded");
        }

        public T GetExtension<T>(string id)
            where T : AppExtension
        {
            var entry = entries.Find(ext => ext.Id == id);
            if (entry == null)
                return null;
            return entry.Extension as T;
        }
        
        public void Dispose()
        {
            if (_disposed)
                return;
            UnloadExtensions();
            serializerSettings = null;
            entries = null;
            localRespository = null;
            _disposed = true;
        }

        /// <summary>
        /// Unloads all loaded application extensions
        /// </summary>
        private void UnloadExtensions()
        {
            //appLogger.PushDebug(null, "Unloading extensions");
            //ExtensionsInit init = new ExtensionsInit();
            //LinkedList<RegisterItem> registerItems = new LinkedList<RegisterItem>();
            //SortExtensions();
            //foreach (var entry in entries)
            //{
            //    if (entry.PendingUninstall)
            //    {
            //        AppKernel.Instance.AddTask(new DeletePathTask(TaskActivationEvent.BeforeLoad, TaskRepetitionMode.Once)
            //        {
            //            Path = fileManager.Combine(fileManager[FileManager.EXTENSIONS], entry.Origin)
            //        });
            //        continue;
            //    }
            //    registerItems.AddLast(new RegisterItem()
            //    {
            //        Id = entry.Id,
            //        Enabled = entry.IsEnabled,
            //        Path = entry.Origin
            //    });
            //    entry.Extension.Unload();
            //}
            //init.Items = registerItems.ToArray();
            //string directory = fileManager.GetPath(FileManager.EXTENSIONS);
            //string serializeData = JsonConvert.SerializeObject(init, Formatting.Indented);
            //fileManager.Write(serializeData, fileManager.Combine(directory, "init.json"));
            //entries.Clear();
            //appLogger.PushDebug(null, "Extensions unloaded");
        }
        private void LoadExtension(string root, IPackage package)
        {
            string extensionPath = localPathResolver.GetInstallPath(package);
            string configData = fileManager.TryRead(fileManager.Combine(extensionPath, "config.json"));
            if (configData == null)
            {
                appLogger.PushWarning($"Extension '{package.Id}' does not have config.json file");
                return;
            }
            var config = JsonConvert.DeserializeObject<ExtensionConfig>(configData);
            if (config.PreloadedAssemblies != null)
            {
                foreach (var preloaded in config.PreloadedAssemblies)
                {
                    string file = fileManager.Combine(extensionPath, preloaded);
                    Assembly.LoadFrom(file);
                }
            }
            Assembly assembly = Assembly.LoadFrom(fileManager.Combine(extensionPath, config.DllPath));
            var types = assembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                bool isExtension = types[i].IsSubclassOf(typeof(AppExtension));
                if (isExtension)
                {
                    var instance = Activator.CreateInstance(types[i], ToSafeId(package.Id), config.IsEnabled) as AppExtension;
                    AppExtensionEntry extensionEntry = new AppExtensionEntry(instance)
                    {
                        IsEnabled = config.IsEnabled,
                        Description = package.Description,
                        Version = package.Version.Version,
                        Origin = package.ProjectUrl.ToString(),
                        Authors = package.Authors,
                        Copytight = package.Copyright,
                        LiceseUrl = package.LicenseUrl,
                        Owners = package.Owners,
                        ProjectUrl = package.ProjectUrl,
                        Tags = package.Tags.Split(' '),
                        Title = package.Title,
                        Dependencies = package.DependencySets.SelectMany(dps => dps.Dependencies)
                                                             .Select(dp => dp.ToString()).ToArray()
                    };
                    RegisterExtension(extensionEntry);
                }
            }
            foreach (var member in config.ExtensionMembers)
            {
                if (member.Platform != Kernel.UIManager.CurrentPlatform)
                    continue;
                if (member.Target != MemberTarget.UI)
                    continue;
                if (member.Role == MemberRole.Extension)
                {
                    string ext = fileManager.Combine(root, extensionPath, member.Path);
                    Kernel.UIManager.RegisterUIExtension(ext);
                }
            }
        }
        private void RegisterExtension(AppExtensionEntry entry)
        {
            appLogger.PushDebug(null, $"Registering extension '{entry.Id}'");
            try
            {
                if (entry == null)
                    throw new ArgumentNullException("Extension parameter can not be null");
                if (entries.FirstOrDefault(e => e.Id == entry.Id) != null)
                    throw new ArgumentException("Duplicate extension ID");
            }
            catch (Exception ex)
            {
                appLogger.PushError(ex, entry);
                return;
            }
            entry.Extension.Initialize();
            entries.Add(entry);
            appLogger.PushDebug(null, $"Extension '{entry.Id}' registered");
        }

        private void OnSerializationError(object sender, ErrorEventArgs e)
        {
            Kernel.AppLogger.PushError(e.ErrorContext.Error, e.ErrorContext, 
                                       $"An error occurred on object serialization: {e.ErrorContext.Error.Message}");
        }
    }

    public class AppExtensionEntry
    {
        public string Id { get; }
        /// <summary>
        /// A flag to indicate whether the extension is enabled to load
        /// </summary>
        public bool IsEnabled { get; internal set; }
        /// <summary>
        /// A flag to indicate entry to uninstall from application
        /// </summary>
        public bool PendingUninstall { get; internal set; }
        public string Title { get; internal set; }
        /// <summary>
        /// A short description of the extension
        /// </summary>
        public string Description { get; internal set; }
        public string Copytight { get; internal set; }        
        /// <summary>
        /// Original extension path from which it was loaded
        /// </summary>
        public string Origin { get; internal set; }
        public Uri LiceseUrl { get; internal set; }
        public Uri ProjectUrl { get; internal set; }
        public Version Version { get; internal set; }
        public string[] Tags { get; internal set; }
        /// <summary>
        /// A set of extensions id's this extension depends on
        /// </summary>
        public string[] Dependencies { get; internal set; }        
        public AppExtension Extension { get; }
        public IEnumerable<string> Owners { get; internal set; }
        public IEnumerable<string> Authors { get; internal set; }

        public AppExtensionEntry(AppExtension extension)
        {
            Id = extension.Id;
            Extension = extension;
            IsEnabled = extension.IsEnabled;
        }
    }
}