//using NuGet;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Vardirsoft.Shared.Helpers;

using Vardirsoft.XApp.API;
using Vardirsoft.XApp.IoC;
using Vardirsoft.XApp.Logging;
using Vardirsoft.XApp.Components;
using Vardirsoft.XApp.FileSystem;
using Vardirsoft.XApp.Extensibility;
using Vardirsoft.XApp.Application.Configuration;

namespace Vardirsoft.XApp.Application
{
    /// <summary>
    /// Application extension manager that helps installing, enabling/disabling and uninstalling extensions
    /// </summary>
    public sealed class ExtensionsManager : ApplicationComponent, IDisposable
    {
        private bool _disposed;
        private JsonSerializerSettings _serializerSettings;
        //private IPackageRepository localRespository;
        //private DefaultPackagePathResolver localPathResolver;
        private readonly LinkedList<AppExtensionEntry> _pendingRegister;
        private Dictionary<string, AppExtensionEntry> _registered;

        /// <summary>
        /// A set of registered application extensions
        /// </summary>
        public IEnumerable<AppExtensionEntry> Extensions
        {
            [DebuggerStepThrough] get => _registered.Select(kvp => kvp.Value);
        }

        public ExtensionsManager() : base("app-extension-manager")
        {
            _serializerSettings = new JsonSerializerSettings { Error = OnSerializationError };
            _pendingRegister = new LinkedList<AppExtensionEntry>();
            _registered = new Dictionary<string, AppExtensionEntry>();
        }

        /// <summary>
        /// Loads all the extensions that are provided in attached assemblies
        /// </summary>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="ApplicationException"></exception>
        public void LoadExtensions()
        {
            var logger = IoCContainer.Resolve<ILogger>();
            logger.PushDebug(null, "Loading extensions");

            var fileManager = IoCContainer.Resolve<IFileManager>();
            var directory = fileManager.GetAbsolutePath(ApplicationPaths.Instance[ApplicationPaths.EXTENSIONS]);
            //localRespository = PackageRepositoryFactory.Default.CreateRepository(directory);
            //localPathResolver = new DefaultPackagePathResolver(directory);
            var lines = fileManager.TryReadLines(fileManager.Combine(directory, ".enabled"));
            var enabledExtensions = new HashSet<string>(lines?.Where(l => !string.IsNullOrWhiteSpace(l)) ?? new string[]{});
            //foreach (var pack in localRespository.GetPackages())
            //{
            //    string error = Validate(pack);
            //    if (error.HasValue())
            //    {
            //        appLogger.PushWarning($"Couldn't load extension '{pack.Id}': {error}");
            //        continue;
            //    }
            //    var entry = new AppExtensionEntry(pack, localPathResolver.GetInstallPath(pack), enabledExtensions.Contains(pack.Id));
            //    pendingRegister.AddLast(entry);
            //}
            RegisterExtensions();
            logger.PushDebug(null, "Extensions loaded");
        }

        //public static string VerifyPackageFrameworkDependencies(IPackage package)
        //{
        //    var frameworkAssemblies = package.FrameworkAssemblies.Where(assembly => assembly.AssemblyName[0] == '!').ToList();
        //    foreach (var assembly in frameworkAssemblies)
        //    {
        //        var (name, part) = assembly.AssemblyName.ExtractUntil(1, ':');
        //        Version version = Version.Parse(part);
        //        string entry = ConfigurationManager.Instance[name] as string;
        //        Version version2 = entry.HasValue() ? Version.Parse(entry) : null;
        //        if (version2 is null)
        //            return $"Application does not meet extension's requirements on '{name}' of {version} version";
        //        if (version2 < version)
        //            return $"Extension requires '{name}' of {version} version but got {version2}";
        //    }
        //    return null;
        //}

        public T GetExtension<T>(string id)
            where T : AppExtension
        {
            if (_registered.TryGetValue(id, out var entry))
                return entry.Extension as T;

            return null;
        }

        public void Dispose()
        {
            if (_disposed)
                return;
                
            UnloadExtensions();
            _serializerSettings = null;
            _registered = null;
            //localRespository = null;
            _disposed = true;
        }

        /// <summary>
        /// Unloads all loaded application extensions
        /// </summary>
        private void UnloadExtensions()
        {
            var logger = IoCContainer.Resolve<ILogger>();
            logger.PushDebug(null, "Unloading extensions");

            var fileManager = IoCContainer.Resolve<IFileManager>();
            var enabled = _registered.Select(kvp => kvp.Value)
                                    .Where(e => e.IsEnabled && !e.PendingUninstall)
                                    .Select(e => e.Id);
            var directory = ApplicationPaths.Instance[ApplicationPaths.EXTENSIONS];

            fileManager.Write(enabled, fileManager.Combine(directory, ".enabled"));
            foreach (var kvp in _registered)
            {
                kvp.Value.Extension.Unload();
            }

            _registered.Clear();

            logger.PushDebug(null, "Extensions unloaded");
        }
        private void RegisterExtensions()
        {
            var node = _pendingRegister.First;
            while (node.HasValue())
            {
                var entry = node.Value;
                var next = node.Next;

                _pendingRegister.Remove(node);

                var resolved = true;
                foreach (var dependency in entry.Dependencies)
                {
                    if (_registered.TryGetValue(dependency, out var parent))
                    {
                        parent.AddDependent(entry.Id);
                        
                        if (!parent.IsEnabled)
                            entry.IsEnabled = false;

                        continue;
                    }

                    resolved = false;
                    _pendingRegister.AddLast(node);

                    break;
                }

                node = next;

                if (resolved)
                    RegisterExtension(entry);
            }

            foreach (var entry in _pendingRegister)
            {
                IoCContainer.Resolve<ILogger>().PushWarning($"Couldn't register extension '{entry.Id}' because one of it dependencies wasn't loaded");
            }

            _pendingRegister.Clear();
        }
        private void RegisterExtension(AppExtensionEntry entry)
        {
            var logger = IoCContainer.Resolve<ILogger>();
            logger.PushDebug(null, $"Registering extension '{entry.Id}'");

            try
            {
                if (entry is null)
                    throw new ArgumentNullException("Extension parameter can not be null");

                if (_registered.ContainsKey(entry.Id))
                    throw new ArgumentException("Duplicate extension ID");

                LoadExtension(entry);
            }
            catch (Exception ex)
            {
                logger.PushError(ex, entry);
                return;
            }

            entry.Extension.Initialize();
            _registered.Add(entry.Id, entry);
            logger.PushDebug(null, $"Extension '{entry.Id}' registered");
        }
        private void LoadExtension(AppExtensionEntry entry)
        {
            var fileManager = IoCContainer.Resolve<IFileManager>();
            var configurationManager = IoCContainer.Resolve<ConfigurationManager>();
            var file = fileManager.Combine(entry.Origin, $"{entry.Id}.dll");

            if (fileManager.FileExists(file))
            {
                var assembly = Assembly.LoadFrom(file);
                var types = assembly.GetTypes();
                for (var i = 0; i < types.Length; i++)
                {
                    var isExtension = types[i].IsSubclassOf(typeof(AppExtension));
                    if (isExtension)
                    {
                        var instance = Activator.CreateInstance(types[i], ToSafeId(entry.Id), entry.IsEnabled) as AppExtension;
                        entry.Extension = instance;
                    }
                }

                if (entry.Extension is null)
                    throw new EntryPointNotFoundException("Extension kernel library does not have suitable extension definition");

                file = fileManager.Combine(entry.Origin, "lib", configurationManager["platform"], $"{entry.Id}.UI.dll");
                if (fileManager.FileExists(file))
                    IoCContainer.Resolve<UIManager>().RegisterUIExtension(file);

                file = fileManager.Combine(entry.Origin, "assets", "lang", $"{configurationManager["lang"]}.json");
                Localization.LoadFrom(file);
            }
            
            throw new DllNotFoundException("Extension does not have a kernel library");
        }

        private void OnSerializationError(object sender, ErrorEventArgs e)
        {
            IoCContainer.Resolve<ILogger>().PushError(e.ErrorContext.Error, e.ErrorContext, 
                                                      $"An error occurred on object serialization: {e.ErrorContext.Error.Message}");
        }

        //private string Validate(IPackage package)
        //{
        //    string error = VerifyPackageFrameworkDependencies(package);
        //    if (error.HasValue())
        //        return error;
        //    return null;
        //}
    }
    
    public class AppExtensionEntry
    {
        private HashSet<string> _dependants;
        private HashSet<string> _dependencies;

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
        
        public int DependantsCount { [DebuggerStepThrough] get => _dependants.Count; }
        
        public int DependenciesCount { [DebuggerStepThrough] get => _dependencies.Count; }
        
        /// <summary>
        /// A set of extensions id's this extension depends on
        /// </summary>
        public IEnumerable<string> Dependencies { [DebuggerStepThrough] get => _dependencies; }
        
        /// <summary>
        /// A set of extensions that are depend on this extension
        /// </summary>
        public IEnumerable<string> Dependants { [DebuggerStepThrough] get => _dependants; }
        
        public IEnumerable<string> Owners { get; }
        
        public IEnumerable<string> Authors { get; }
        
        public AppExtension Extension { get; internal set; }

        //public AppExtensionEntry(IPackage package, string location, bool isEnabled = true)
        //{
        //    Id = package.Id;
        //    Origin = location;
        //    IsEnabled = isEnabled;
        //    Title = package.Title ?? Id;
        //    Copyright = package.Copyright;
        //    LiceseUrl = package.LicenseUrl;
        //    Tags = package.Tags.Split(' ');
        //    ProjectUrl = package.ProjectUrl;
        //    Description = package.Description;
        //    Version = package.Version.Version;
        //    dependants = new HashSet<string>();
        //    dependencies = new HashSet<string>(package.DependencySets.SelectMany(set => set.Dependencies)
        //                                                             .Select(dp => dp.Id));
        //}

        public void AddDependent(string id) => _dependants.Add(id);
    }
}