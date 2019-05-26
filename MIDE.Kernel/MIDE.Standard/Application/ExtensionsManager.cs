using System;
using System.Linq;
using MIDE.Helpers;
using Newtonsoft.Json;
using MIDE.FileSystem;
using MIDE.Schemes.JSON;
using System.Reflection;
using MIDE.API.Components;
using MIDE.Application.Tasks;
using MIDE.API.Extensibility;
using MIDE.Application.Logging;
using System.Collections.Generic;
using MIDE.Application.Attrubites;
using Newtonsoft.Json.Serialization;
using NuGet;

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

        /// <summary>
        /// A set of registered application extensions
        /// </summary>
        public IEnumerable<AppExtension> Extensions => entries.Select(axe => axe.Extension);
        public IEnumerable<AppExtensionEntry> ExtensionsEntries => entries;

        private ExtensionsManager() : base("app-extension-manager")
        {
            appLogger = Kernel.AppLogger;
            fileManager = FileManager.Instance;
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
            var directory = fileManager.GetPath(FileManager.EXTENSIONS);
            var initData = fileManager.ReadOrCreate(fileManager.Combine(directory, "init.json"), "{ \"register\": [] }");
            var init = JsonConvert.DeserializeObject<ExtensionsInit>(initData);
            foreach (var item in init.Items)
            {
                LoadExtension(directory, item.Path, item.Id, item.Enabled);
            }
            appLogger.PushDebug(null, "Extensions loaded");
        }

        /// <summary>
        /// Searches for an extension with the given ID on the specified repository.
        /// Changes applied after application restart. Returns error result message, null if success
        /// </summary>
        /// <param name="path"></param>
        public string Install(string repositoryPath, string file)
        {
            string directory = fileManager.GetPath(FileManager.EXTENSIONS);
            var repository = PackageRepositoryFactory.Default.CreateRepository(repositoryPath);
            if (repository == null)
                return $"Can not install extension: repository '{repositoryPath}' not found";
            string id = null;
            foreach (var item in repository.GetPackages())
            {
                string fullName = $"{item.Id}.{item.Version}";
                if (fullName == file)
                {
                    id = item.Id;
                    break;
                }
            }
            if (id == null)
                return $"Can not install extension: could not found extension '{file}' on repository {repositoryPath}";

            PackageManager packageManager = new PackageManager(repository, directory);
            packageManager.InstallPackage(id);
            LoadExtension(directory, file, id, false);
            return null;
        }
        /// <summary>
        /// Finds a previously installed extension by the given name and uninstalls it from default application extension's path. 
        /// Changes applied after application restart. Returns error result message, null if success
        /// </summary>
        /// <param name="path"></param>
        public string Uninstall(string id)
        {
            var entry = entries.FirstOrDefault(ax => ax.Id == id);
            if (entry == null)
                return "Can not uninstall extension that does not installed or loaded yet";

            entry.PendingUninstall = true;
            return null;
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
            _disposed = true;
        }

        /// <summary>
        /// Unloads all loaded application extensions
        /// </summary>
        private void UnloadExtensions()
        {
            appLogger.PushDebug(null, "Unloading extensions");
            ExtensionsInit init = new ExtensionsInit();
            LinkedList<RegisterItem> registerItems = new LinkedList<RegisterItem>();
            SortExtensions();
            foreach (var entry in entries)
            {
                if (entry.PendingUninstall)
                {
                    AppKernel.Instance.AddTask(new DeletePathTask(TaskActivationEvent.BeforeLoad, TaskRepetitionMode.Once)
                    {
                        Path = fileManager.Combine(fileManager[FileManager.EXTENSIONS], entry.Origin)
                    });
                    continue;
                }
                registerItems.AddLast(new RegisterItem()
                {
                    Id = entry.Id,
                    Enabled = entry.IsEnabled,
                    Path = entry.Origin
                });
                entry.Extension.Unload();
            }
            init.Items = registerItems.ToArray();
            string directory = fileManager.GetPath(FileManager.EXTENSIONS);
            string serializeData = JsonConvert.SerializeObject(init, Formatting.Indented);
            fileManager.Write(serializeData, fileManager.Combine(directory, "init.json"));
            entries.Clear();
            appLogger.PushDebug(null, "Extensions unloaded");
        }
        private void LoadExtension(string root, string path, string id, bool isEnabled)
        {
            string extensionPath = fileManager.Combine(fileManager.GetPath(FileManager.EXTENSIONS), path);
            string configData = fileManager.TryRead(fileManager.Combine(extensionPath, "config.json"));
            if (configData == null)
            {
                appLogger.PushWarning($"Extension '{id}' does not have config.json file");
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
                    var instance = Activator.CreateInstance(types[i], id, isEnabled) as AppExtension;
                    AppExtensionEntry extensionEntry = new AppExtensionEntry(instance);
                    extensionEntry.IsEnabled = isEnabled;
                    extensionEntry.Description = config.Description;
                    extensionEntry.Origin = path;
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
                    string ext = fileManager.Combine(root, path, member.Path);
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
                var type = entry.Extension.GetType();
                string error = VerifyExtensionAttributes(type, entry);
                if (error != null)
                    throw new InvalidOperationException($"Extension [{entry.GetType()} :: {entry.Id}] can not be registered due to error: {error}");
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

        private void SortExtensions(int startingIndex = 0)
        {
            if (entries.Count == 0)
                return;
            for (int i = startingIndex; i < entries.Count; i++)
            {
                AppExtensionEntry ext = entries[i];
                for (int j = 0; j < ext.Dependencies.Length; j++)
                {
                    int index = entries.IndexOf(ax => ax.Id == ext.Dependencies[j], i);
                    if (index == -1)
                        continue;
                    entries.MoveTo(i, index);
                    i = 0;
                    break;
                }
            }
        }
        private void OnSerializationError(object sender, ErrorEventArgs e)
        {
            Kernel.AppLogger.PushError(e.ErrorContext.Error, e.ErrorContext, 
                                       $"An error occurred on object serialization: {e.ErrorContext.Error.Message}");
        }

        private string VerifyExtensionAttributes(Type type, AppExtensionEntry entry)
        {
            appLogger.PushDebug(null, $"Verifying extension attributes for {entry.Id} of type {type}");
            bool kernelVerified = false;
            LinkedList<string> dependencies = new LinkedList<string>();
            foreach (var attribute in type.GetCustomAttributes(false))
            {
                if (attribute is ExtensionPropertiesAttribute properties)
                {
                    entry.Version = properties.Version;
                }
                else if (attribute is DependencyAttribute dependency)
                {
                    if (dependency.Type == DependencyType.ApplicationKernel)
                    {
                        if (kernelVerified)
                            return $"Duplicate DependencyAttribute entry of type ApplicationKernel on extension [{type} :: ID-{entry.Id}]";
                        if (dependency.Version != Kernel.Version)
                            return $"Extension [{type} :: {entry.Id}] is targeting Kernel version {dependency.Version} but current is {Kernel.Version}";
                        kernelVerified = true;
                    }
                    else if (dependency.Type == DependencyType.Extension)
                    {
                        var any = entries.FirstOrDefault(e => e.GetType().Name == dependency.DependentOn);
                        if (any == null)
                            return $"Extension [{type} :: {entry.Id}] requires an extension [{dependency.DependentOn}]";
                        if (any.Version != dependency.Version)
                            return $"Extension [{type} :: {entry.Id}] requires an extension [{dependency.DependentOn}] v{dependency.Version} but got v{any.Version}";
                        dependencies.AddLast(any.Id);
                    }
                }
            }
            if (!kernelVerified)
                return $"An expected DependencyAttribute of type ApplicationKernel not found on extension [{type} :: ID-{entry.Id}]";
            entry.Dependencies = dependencies.ToArray();
            appLogger.PushDebug(null, "Extension attributes verified");
            return null;
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
        /// <summary>
        /// A short description of the extension
        /// </summary>
        public string Description { get; internal set; }
        /// <summary>
        /// Version of the extension
        /// </summary>
        public string Version { get; internal set; }
        /// <summary>
        /// Original extension path from which it was loaded
        /// </summary>
        public string Origin { get; internal set; }
        /// <summary>
        /// A set of extensions id's this extension depends on
        /// </summary>
        public string[] Dependencies { get; internal set; }        
        public AppExtension Extension { get; }

        public AppExtensionEntry(AppExtension extension)
        {
            Id = extension.Id;
            Extension = extension;
            IsEnabled = extension.IsEnabled;
        }
    }
}