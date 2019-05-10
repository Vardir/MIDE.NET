using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using MIDE.FileSystem;
using System.Reflection;
using MIDE.API.Services;
using MIDE.Schemes.JSON;
using MIDE.API.Extensibility;
using MIDE.Application.Logging;
using System.Collections.Generic;
using MIDE.Application.Attrubites;
using MIDE.Application.Initializers;
using MIDE.Application.Configuration;
using Module = MIDE.API.Extensibility.Module;
using MIDE.Application.Events;

namespace MIDE.Application
{
    public class AppKernel : IDisposable
    {
        private static AppKernel instance;
        public static AppKernel Instance => instance ?? (instance = new AppKernel());

        private bool isRunning;
        private Assembly currentAssembly;
        private Assembly callingAssembly;
        private readonly List<AppExtension> extensions;

        /// <summary>
        /// A time when application kernel was started
        /// </summary>
        public DateTime TimeStarted { get; private set; }
        /// <summary>
        /// Version of application kernel
        /// </summary>
        public string Version { get; }
        /// <summary>
        /// Name of application kernel
        /// </summary>
        public string ApplicationName { get; private set; }
        /// <summary>
        /// Application-wide logger
        /// </summary>
        public Logger AppLogger { get; }
        /// <summary>
        /// Application UI manager
        /// </summary>
        public UIManager UIManager { get; set; }
        /// <summary>
        /// File manager used by application to create/load files and folders
        /// </summary>
        public FileManager FileManager { get; set; }
        /// <summary>
        /// Application-wide event broadcaster to provide event-based interaction between application components
        /// </summary>
        public EventBroadcaster Broadcaster { get; }
        /// <summary>
        /// System clipboard interface that helps interact between application and system
        /// </summary>
        public IClipboardProvider SystemClipboard { get; set; }
        /// <summary>
        /// A set of extensions
        /// </summary>
        public IEnumerable<AppExtension> Extensions => extensions;
        /// <summary>
        /// List of application initializers used to configure kernel and it's parts
        /// </summary>
        public List<IApplicationInitializer> Initializers { get; }

        public event Action ApplicationExit;

        private AppKernel()
        {
            extensions = new List<AppExtension>();
            Initializers = new List<IApplicationInitializer>();

            currentAssembly = Assembly.GetAssembly(typeof(AppKernel));
            var version = currentAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            Version = version.InformationalVersion;

            AppLogger = new Logger(LoggingLevel.ALL, useUtcTime: true, skipFatals: false);
            AppLogger.FatalEventRegistered += AppLogger_FatalEventRegistered;

            AppLogger.PushDebug(null, "Application Kernel created");
        }

        /// <summary>
        /// Starts the application kernel
        /// </summary>
        public void Start()
        {
            AppLogger.PushDebug(null, "Application Kernel starting");
            try
            {
                if (isRunning)
                    throw new ApplicationException("Application kernel is already loaded and running!");
                if (FileManager == null)
                    throw new NullReferenceException("The FileManager expected to be assigned before application start");
                if (SystemClipboard == null)
                    throw new NullReferenceException("The SystemBuffer expected to be assigned before application start");
                if (UIManager == null)
                    throw new NullReferenceException("The UIHandler expected to be assigned before application start");
                callingAssembly = Assembly.GetCallingAssembly();
                string assemblyVertification = VerifyAssemblyAttributes();
                if (assemblyVertification != null)
                    throw new ApplicationException(assemblyVertification);
            }
            catch (Exception ex)
            {
                isRunning = true;
                AppLogger.PushFatal(ex.Message);
            }

            AppLogger.PushDebug(null, "Application Kernel started");
            TimeStarted = DateTime.UtcNow;
            isRunning = true;
            foreach (var initializer in Initializers)
            {
                initializer.Execute(this);
            }
            LoadConfigurations();
            try
            {
                LoadExtensions();
            }
            catch (Exception ex)
            {
                AppLogger.PushError(ex, this);
            }
        }
        /// <summary>
        /// Saves current session's log entries
        /// </summary>
        public void SaveLog()
        {
            if (AppLogger.EventsCount == 0)
                return;
            string folder = $"{FileManager.GetPath(ApplicationPath.Logs)}\\{TimeStarted.ToString("dd-M-yyyy HH-mm-ss")}\\";
            string filename = $"{folder}log.txt";
            FileManager.MakeFolder(folder);
            StringBuilder builder = new StringBuilder();
            builder.Append(ApplicationName);
            builder.Append(' ');
            builder.Append(Version);
            builder.AppendLine();
            builder.Append("Is UTC time = " + AppLogger.UseUtcTime);
            builder.AppendLine();
            builder.Append("-------------------------");
            builder.AppendLine();
            foreach (var item in AppLogger.Pull())
            {
                builder.Append(item.ToString());
                object[] serializationData = item.GetSerializationData();
                if (serializationData != null)
                {
                    for (int i = 0; i < serializationData.Length; i++)
                    {
                        if (serializationData[i] == null)
                            continue;
                        builder.Append("  - [");
                        builder.Append(i + 1);
                        builder.Append(".bin] type - ");
                        builder.Append(serializationData[i].GetType());
                        builder.AppendLine();
                        FileManager.Serialize(serializationData[i], $"{folder}{i + 1}.bin");
                    }
                }
                builder.AppendLine();
            }
            FileManager.Write(builder.ToString(), filename);
        }
        /// <summary>
        /// Stops all the current threads, releases all resources and closes the application kernel
        /// </summary>
        /// <exception cref="ApplicationException"></exception>
        public void Exit()
        {
            if (!isRunning)
            {
                AppLogger.PushWarning("Attempt to terminate application while it is not started yet");
                return;
            }
            AppLogger.PushDebug(null, "Application Kernel stopped");
            isRunning = false;
            ClearTemporaryFiles();
            Dispose();
            AppLogger.PushDebug(null, "Application Kernel resources are disposed");
            SaveLog();
            ApplicationExit?.Invoke();
        }

        /// <summary>
        /// Verifies if the module is valid for the current application.
        /// Returns null if valid, otherwise returns message.
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public string VerifyModule(Module module)
        {
            //TODO: verify if the module is valid
            return null;
        }
        public override string ToString() => $"KERNEL [{Version}] >> {callingAssembly?.FullName ?? "?"}";

        /// <summary>
        /// Registers the given extension in the internal storage and initializes it
        /// </summary>
        /// <param name="extension"></param>
        public void RegisterExtension(AppExtension extension)
        {
            AppLogger.PushDebug(null, $"Registering extension {extension?.Id ?? "<null>"}");
            try
            {
                if (extension == null)
                    throw new ArgumentNullException("Extension parameter can not be null");
                if (extensions.FirstOrDefault(e => e.Id == extension.Id) != null)
                    throw new ArgumentException("Duplicate extension ID");
                var type = extension.GetType();
                string error = VerifyExtensionAttributes(type, extension.Id);
                if (error != null)
                    throw new InvalidOperationException($"Extension [{extension.GetType()} :: {extension.Id}] can not be registered due to error: {error}");
            }
            catch (Exception ex)
            {
                AppLogger.PushError(ex, extension);
                return;
            }
            extension.Initialize();
            extensions.Add(extension);
            AppLogger.PushDebug(null, $"Registered extension {extension.Id}");
        }

        public void Dispose()
        {
            if (isRunning)
            {
                AppLogger.PushWarning("Attempt to dispose of resources while application still running");
                return;
            }
            //TODO: dispose all the application resources
            UnloadExtensions();
        }

        /// <summary>
        /// Loads all the configurations that are set in config.json file located in the same folder with application itself
        /// </summary>
        /// <returns></returns>
        private void LoadConfigurations()
        {
            AppLogger.PushDebug(null, "Loading application configurations");
            ApplicationConfig appConfig = null;
            try
            {
                string configData = FileManager.ReadOrCreate("config.json", $"{{ \"kernel_version\": \"{Version}\"}}");
                appConfig = JsonConvert.DeserializeObject<ApplicationConfig>(configData);
                if (appConfig.KernelVersion != Version)
                    throw new ApplicationException($"Expected application kernel v{Version}, but got v{appConfig.KernelVersion}");
            }
            catch (Exception ex)
            {
                AppLogger.PushFatal(ex.Message);
            }
            if (appConfig.LoggingLevels == null || appConfig.LoggingLevels.Length == 0)
                AppLogger.Levels = LoggingLevel.ALL;
            else
                AppLogger.Levels = appConfig.LoggingLevels.Aggregate((l1, l2) => l1 | l2);
            AppLogger.FilterEvents(AppLogger.Levels);
            ConfigurationManager.Instance.AddOrUpdate(new Config("theme", appConfig.Theme));
            FileManager.LoadPaths(appConfig.Paths);
            AppLogger.PushDebug(null, "Application configurations loaded");
        }

        /// <summary>
        /// Loads all the extensions that are provided in attached assemblies
        /// </summary>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="ApplicationException"></exception>
        private void LoadExtensions()
        {
            AppLogger.PushDebug(null, "Loading extensions");
            var directory = FileManager.GetOrAddPath(ApplicationPath.Extensions, "extensions\\");
            var initData = FileManager.ReadOrCreate(directory + "init.json", "{ \"register\": [] }");
           
            var init = JsonConvert.DeserializeObject<ExtensionsInit>(initData);
            foreach (var item in init.Items)
            {
                if (!item.Enabled)
                    continue;
                string extensionPath = FileManager.GetPath(ApplicationPath.Extensions) + "\\" + item.Path;
                string configData = FileManager.TryRead(extensionPath + "config.json");
                if (configData == null)
                {
                    AppLogger.PushWarning($"Extension '{item.Id}' does not have config.json file");
                    continue;
                }
                var config = JsonConvert.DeserializeObject<ExtensionConfig>(configData);
                Assembly assembly = Assembly.LoadFrom(extensionPath + config.DllPath);
                var types = assembly.GetTypes();
                for (int i = 0; i < types.Length; i++)
                {
                    bool isExtension = types[i].IsSubclassOf(typeof(AppExtension));
                    if (isExtension)
                    {
                        var instance = Activator.CreateInstance(types[i], item.Id) as AppExtension;
                        RegisterExtension(instance);
                    }
                }
                foreach (var member in config.ExtensionMembers)
                {
                    if (member.Platform != UIManager.CurrentPlatform)
                        continue;
                    if (member.Target != MemberTarget.UI)
                        continue;                    
                    if (member.Role == MemberRole.Extension)
                    {
                        string path = FileManager.Combine(directory, item.Path, member.Path);
                        UIManager.RegisterUIExtension(path);
                    }
                }
            }
            AppLogger.PushDebug(null, "Extensions loaded");
        }
        /// <summary>
        /// Unload all the resources that are used by application extensions
        /// </summary>
        private void UnloadExtensions()
        {
            AppLogger.PushDebug(null, "Unloading extensions");
            foreach (var extension in extensions)
            {
                extension.Unload();
            }
            extensions.Clear();
            AppLogger.PushDebug(null, "Extensions unloaded");
        }
        /// <summary>
        /// Cleans off all the temporary files that was in use by the current application session
        /// </summary>
        private void ClearTemporaryFiles()
        {
            AppLogger.PushDebug(null, "Clearing temporary files");
            //TODO: implement cleaning temporary files
            AppLogger.PushDebug(null, "Temporary files cleared");

        }
       
        private string VerifyAssemblyAttributes()
        {
            AppLogger.PushDebug(null, "Verifying assembly attributes");
            bool hasAppPropsAttriburte = false;
            Attribute[] attributes = Attribute.GetCustomAttributes(callingAssembly);
            for (int i = 0; i < attributes.Length; i++)
            {
                Attribute attribute = attributes[i];
                switch (attribute)
                {
                    case ApplicationPropertiesAttribute attr:
                        hasAppPropsAttriburte = true;
                        ApplicationName = attr.ApplicationName;
                        break;
                }
            }

            if (!hasAppPropsAttriburte)
                return "Missing application properties attribute in core assembly file";
            AppLogger.PushDebug(null, "Assembly attributes verified");

            return null;
        }
        private string VerifyExtensionAttributes(Type type, string id)
        {
            AppLogger.PushDebug(null, $"Verifying extension attributes for {id} of type {type}");
            bool kernelVerified = false;
            foreach (var attribute in type.GetCustomAttributes())
            {
                if (attribute is DependencyAttribute dependency)
                {
                    if (dependency.Type == DependencyType.ApplicationKernel)
                    {
                        if (kernelVerified)
                            return $"Duplicate DependencyAttribute entry of type ApplicationKernel on extension [{type} :: ID-{id}]";
                        if (dependency.Version != Version)
                            return $"Extension [{type} :: {id}] is targeting Kernel version {dependency.Version} but current is {Version}";
                        kernelVerified = true;
                    }
                    else if (dependency.Type == DependencyType.Extension)
                    {
                        var any = Extensions.FirstOrDefault(e => e.GetType().Name == dependency.DependentOn);
                        if (any == null)
                            throw new ApplicationException($"Extension [{type} :: {id}] requires an extension [{dependency.DependentOn}]");
                        if (any.Version != dependency.Version)
                            throw new ApplicationException($"Extension [{type} :: {id}] requires an extension [{dependency.DependentOn}] v{dependency.Version} but got v{any.Version}");
                    }
                }
            }
            if (!kernelVerified)
                return $"An expected DependencyAttribute of type ApplicationKernel not found on extension [{type} :: ID-{id}]";
            AppLogger.PushDebug(null, "Extension attributes verified");
            return null;
        }
        
        private void AppLogger_FatalEventRegistered(object sender, FatalEvent e)
        {
            AppLogger.PushInfo("Closing application due to fatal error");
            Exit();
        }
    }
}