using System;
using System.Linq;
using Newtonsoft.Json;
using MIDE.FileSystem;
using System.Reflection;
using MIDE.API.Services;
using MIDE.Schemes.JSON;
using MIDE.API.Extensibility;
using System.Collections.Generic;
using MIDE.Application.Attrubites;
using MIDE.Application.Initializers;
using MIDE.Application.Configuration;
using Module = MIDE.API.Extensibility.Module;

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

        public string Version { get; }
        public string ApplicationName { get; private set; }
        public FileManager FileManager { get; set; }
        public IBufferProvider SystemBuffer { get; set; }
        public IEnumerable<AppExtension> Extensions => extensions;
        public UIManager UIManager { get; set; }
        public List<IApplicationInitializer> Initializers { get; }

        public event Action ApplicationExit;

        private AppKernel()
        {
            UIManager = new UIManager();
            extensions = new List<AppExtension>();
            Initializers = new List<IApplicationInitializer>();

            currentAssembly = Assembly.GetAssembly(typeof(AppKernel));
            var version = currentAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            Version = version.InformationalVersion;
        }

        /// <summary>
        /// Starts the application kernel
        /// </summary>
        /// <exception cref="ApplicationException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public void Start()
        {
            if (isRunning)
                throw new ApplicationException("Application kernel is already loaded and running!");
            if (FileManager == null)
                throw new NullReferenceException("The FileManager expected to be assigned before application start");
            if (SystemBuffer == null)
                throw new NullReferenceException("The SystemBuffer expected to be assigned before application start");
            if (UIManager == null)
                throw new NullReferenceException("The UIHandler expected to be assigned before application start");
            callingAssembly = Assembly.GetCallingAssembly();
            string assemblyVertification = VerifyAssemblyAttributes();
            if (assemblyVertification != null)
                throw new ApplicationException(assemblyVertification);

            isRunning = true;
            //TODO: prepare application
            foreach (var initializer in Initializers)
            {
                initializer.Execute(this);
            }
            var configs = LoadConfigurations();
            ConfigurationManager.Instance.AddRange(configs);
            LoadExtensions();
        }        
        /// <summary>
        /// Stops all the current threads, releases all resources and closes the application kernel
        /// </summary>
        /// <exception cref="ApplicationException"></exception>
        public void Exit()
        {
            if (!isRunning)
                throw new ApplicationException("Can not exit application that is not started");
            isRunning = false;
            ClearTemporaryFiles();
            Dispose();
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
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void RegisterExtension(AppExtension extension)
        {
            if (extension == null)
                throw new ArgumentNullException("Extension parameter can not be null");
            if (extensions.FirstOrDefault(e => e.Id == extension.Id) != null)
                throw new ArgumentException("Duplicate extension ID");
            var type = extension.GetType();
            string error = VerifyExtensionAttributes(type, extension.Id);
            if (error != null)
                throw new InvalidOperationException($"Extension [{extension.GetType()} :: {extension.Id}] can not be registered due to error: {error}");
            extension.Kernel = this;
            extension.Initialize();
            extensions.Add(extension);
        }
        
        /// <summary>
        /// Loads all the configurations that are set in config.json file located in the same folder with application itself
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Config> LoadConfigurations()
        {
            string configData = FileManager.ReadOrCreate("config.json", $"{{ \"core_version\": \"{Version}\"}}");
            var appConfig = JsonConvert.DeserializeObject<ApplicationConfig>(configData);
            if (appConfig.KernelVersion != Version)
                throw new ApplicationException($"Expected application kernel v{Version}, but got v{appConfig.KernelVersion}");
            FileManager.LoadPaths(appConfig.Paths);
            yield break;
        }
        
        /// <summary>
        /// Loads all the extensions that are provided in attached assemblies
        /// </summary>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="ApplicationException"></exception>
        private void LoadExtensions()
        {
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
                    throw new ApplicationException($"Extension '{item.Id}' does not have config.json file");
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
                    //TODO: load members
                }
            }
        }
        /// <summary>
        /// Unload all the resources that are used by application extensions
        /// </summary>
        private void UnloadExtensions()
        {
            foreach (var extension in extensions)
            {
                extension.Unload();
                extension.Kernel = null;
            }
            extensions.Clear();
        }
        /// <summary>
        /// Cleans off all the temporary files that was in use by the current application session
        /// </summary>
        private void ClearTemporaryFiles()
        {

        }
       
        private string VerifyAssemblyAttributes()
        {
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
                throw new ApplicationException($"Missing application properties attribute [{typeof(ApplicationPropertiesAttribute)}]");

            return null;
        }
        private string VerifyExtensionAttributes(Type type, string id)
        {
            bool coreVerified = false;
            foreach (var attribute in type.GetCustomAttributes())
            {
                if (attribute is DependencyAttribute dependency)
                {
                    if (dependency.Type == DependencyType.ApplicationKernel)
                    {
                        if (coreVerified)
                            return $"Duplicate DependencyAttribute entry of type ApplicationKernel on extension [{type} :: ID-{id}]";
                        if (dependency.Version != Version)
                            return $"Extension [{type} :: {id}] is targeting Kernel version {dependency.Version} but current is {Version}";
                        coreVerified = true;
                    }
                    else if (dependency.Type == DependencyType.Extension)
                    {
                        //TODO: verify if there is registered extension
                        //TODO: verify if the dependent extension has valid version
                    }
                }
            }
            if (!coreVerified)
                return $"An expected DependencyAttribute of type ApplicationKernel not found on extension [{type} :: ID-{id}]";
            return null;
        }

        public void Dispose()
        {
            if (isRunning)
                throw new InvalidOperationException("Can not dispose application resources while it's running");
            //TODO: dispose all the application resources
            UnloadExtensions();
        }
    }
}