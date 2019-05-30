using NuGet;
using System;
using System.Linq;
using MIDE.Helpers;
using Newtonsoft.Json;
using MIDE.FileSystem;
using System.Reflection;
using MIDE.API.Components;
using MIDE.API.Extensibility;
using MIDE.Application.Logging;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using MIDE.Application.Configuration;

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
        private IPackageRepository localRespository;
        private DefaultPackagePathResolver localPathResolver;
        private LinkedList<AppExtensionEntry> pendingRegister;
        private Dictionary<string, AppExtensionEntry> registered;

        /// <summary>
        /// A set of registered application extensions
        /// </summary>
        public IEnumerable<AppExtensionEntry> Extensions => registered.Select(kvp => kvp.Value);

        private ExtensionsManager() : base("app-extension-manager")
        {
            appLogger = Kernel.AppLogger;
            fileManager = FileManager.Instance;
            serializerSettings = new JsonSerializerSettings();
            serializerSettings.Error = OnSerializationError;
            pendingRegister = new LinkedList<AppExtensionEntry>();
            registered = new Dictionary<string, AppExtensionEntry>();
        }

        /// <summary>
        /// Loads all the extensions that are provided in attached assemblies
        /// </summary>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="ApplicationException"></exception>
        public void LoadExtensions()
        {
            appLogger.PushDebug(null, "Loading extensions");
            var directory = fileManager.GetAbsolutePath(ApplicationPaths.Instance[ApplicationPaths.EXTENSIONS]);
            localRespository = PackageRepositoryFactory.Default.CreateRepository(directory);
            localPathResolver = new DefaultPackagePathResolver(directory);
            string[] lines = fileManager.TryReadLines(fileManager.Combine(directory, ".enabled"));
            var enabledExtensions = new HashSet<string>(lines.Where(l => !string.IsNullOrWhiteSpace(l)));
            foreach (var pack in localRespository.GetPackages())
            {
                string error = Validate(pack);
                if (error != null)
                {
                    appLogger.PushWarning($"Couldn't load extension '{pack.Id}': {error}");
                    continue;
                }
                var entry = new AppExtensionEntry(pack, localPathResolver.GetInstallPath(pack), enabledExtensions.Contains(pack.Id));
                pendingRegister.AddLast(entry);
            }
            RegisterExtensions();
            appLogger.PushDebug(null, "Extensions loaded");
        }

        public static string VerifyPackageFrameworkDependencies(IPackage package)
        {
            var frameworkAssemblies = package.FrameworkAssemblies.Where(assembly => assembly.AssemblyName[0] == '!').ToList();
            foreach (var assembly in frameworkAssemblies)
            {
                var (name, part) = assembly.AssemblyName.ExtractUntil(1, ':');
                Version version = Version.Parse(part);
                string entry = ConfigurationManager.Instance[name] as string;
                Version version2 = entry != null ? Version.Parse(entry) : null;
                if (version2 == null)
                    return $"Application does not meet extension's requirements on '{name}' of {version} version";
                if (version2 < version)
                    return $"Extension requires '{name}' of {version} version but got {version2}";
            }
            return null;
        }

        public T GetExtension<T>(string id)
            where T : AppExtension
        {
            if (!registered.TryGetValue(id, out var entry))
                return null;
            return entry.Extension as T;
        }
        
        public void Dispose()
        {
            if (_disposed)
                return;
            UnloadExtensions();
            serializerSettings = null;
            registered = null;
            localRespository = null;
            _disposed = true;
        }

        /// <summary>
        /// Unloads all loaded application extensions
        /// </summary>
        private void UnloadExtensions()
        {
            appLogger.PushDebug(null, "Unloading extensions");
            var enabled = registered.Select(kvp => kvp.Value)
                                    .Where(e => e.IsEnabled && !e.PendingUninstall)
                                    .Select(e => e.Id);
            string directory = ApplicationPaths.Instance[ApplicationPaths.EXTENSIONS];
            fileManager.Write(enabled, fileManager.Combine(directory, ".enabled"));
            foreach (var kvp in registered)
            {
                var entry = kvp.Value;
                entry.Extension.Unload();
            }            
            registered.Clear();
            appLogger.PushDebug(null, "Extensions unloaded");
        }
        private void RegisterExtensions()
        {
            var node = pendingRegister.First;
            while (node != null)
            {
                var entry = node.Value;
                var next = node.Next;
                pendingRegister.Remove(node);
                bool resolved = true;
                foreach (var dependency in entry.Dependencies)
                {
                    if (registered.TryGetValue(dependency, out var parent))
                    {
                        parent.AddDependent(entry.Id);
                        if (!parent.IsEnabled)
                            entry.IsEnabled = false;
                        continue;
                    }
                    resolved = false;
                    pendingRegister.AddLast(node);
                    break;
                }
                node = next;
                if (!resolved)
                    continue;
                RegisterExtension(entry);
            }
            foreach (var entry in pendingRegister)
            {
                appLogger.PushWarning($"Couldn't register extension '{entry.Id}' because one of it dependencies wasn't loaded");
            }
            pendingRegister.Clear();
        }
        private void RegisterExtension(AppExtensionEntry entry)
        {
            appLogger.PushDebug(null, $"Registering extension '{entry.Id}'");
            try
            {
                if (entry == null)
                    throw new ArgumentNullException("Extension parameter can not be null");
                if (registered.ContainsKey(entry.Id))
                    throw new ArgumentException("Duplicate extension ID");
                LoadExtension(entry);
            }
            catch (Exception ex)
            {
                appLogger.PushError(ex, entry);
                return;
            }
            entry.Extension.Initialize();
            registered.Add(entry.Id, entry);
            appLogger.PushDebug(null, $"Extension '{entry.Id}' registered");
        }
        private void LoadExtension(AppExtensionEntry entry)
        {
            string file = fileManager.Combine(entry.Origin, $"{entry.Id}.dll");
            if (!fileManager.FileExists(file))
                throw new DllNotFoundException("Extension does not have a kernel library");

            Assembly assembly = Assembly.LoadFrom(file);
            var types = assembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                bool isExtension = types[i].IsSubclassOf(typeof(AppExtension));
                if (isExtension)
                {
                    var instance = Activator.CreateInstance(types[i], ToSafeId(entry.Id), entry.IsEnabled) as AppExtension;
                    entry.Extension = instance;
                }
            }
            if (entry.Extension == null)
                throw new EntryPointNotFoundException("Extension kernel library does not have suitable extension definition");
            file = fileManager.Combine(entry.Origin, $"{entry.Id}.UI.dll");
            if (fileManager.FileExists(file))
                AppKernel.Instance.UIManager.RegisterUIExtension(file);
        }

        private void OnSerializationError(object sender, ErrorEventArgs e)
        {
            Kernel.AppLogger.PushError(e.ErrorContext.Error, e.ErrorContext, 
                                       $"An error occurred on object serialization: {e.ErrorContext.Error.Message}");
        }

        private string Validate(IPackage package)
        {
            string error = VerifyPackageFrameworkDependencies(package);
            if (error != null)
                return error;
            return null;
        }
    }
    
    public class AppExtensionEntry
    {
        private HashSet<string> dependants;
        private HashSet<string> dependencies;

        public string Id { get; }
        /// <summary>
        /// A flag to indicate whether the extension is enabled to load
        /// </summary>
        public bool IsEnabled { get; internal set; }
        /// <summary>
        /// A flag to indicate entry to uninstall from application
        /// </summary>
        public bool PendingUninstall { get; internal set; }
        /// <summary>
        /// A descriptive name of the application that is visible to end user
        /// </summary>
        public string Title { get; }
        /// <summary>
        /// A short description of the extension
        /// </summary>
        public string Description { get; }
        public string Copyright { get; }        
        /// <summary>
        /// Original extension path from which it was loaded
        /// </summary>
        public string Origin { get; }
        public Uri LiceseUrl { get; }
        public Uri ProjectUrl { get; }
        public Version Version { get; }
        public string[] Tags { get; }
        
        public int DependantsCount => dependants.Count;
        public int DependenciesCount => dependencies.Count;
        /// <summary>
        /// A set of extensions id's this extension depends on
        /// </summary>
        public IEnumerable<string> Dependencies => dependencies;
        /// <summary>
        /// A set of extensions that are depend on this extension
        /// </summary>
        public IEnumerable<string> Dependants => dependants;
        public IEnumerable<string> Owners { get; }
        public IEnumerable<string> Authors { get; }
        public AppExtension Extension { get; internal set; }

        public AppExtensionEntry(IPackage package, string location, bool isEnabled = true)
        {
            Id = package.Id;
            Origin = location;
            IsEnabled = isEnabled;
            Title = package.Title ?? Id;
            Copyright = package.Copyright;
            LiceseUrl = package.LicenseUrl;
            Tags = package.Tags.Split(' ');
            ProjectUrl = package.ProjectUrl;
            Description = package.Description;
            Version = package.Version.Version;
            dependants = new HashSet<string>();
            dependencies = new HashSet<string>(package.DependencySets.SelectMany(set => set.Dependencies)
                                                                     .Select(dp => dp.Id));
        }

        public void AddDependent(string id) => dependants.Add(id);
    }
}