using System;
using System.Linq;
using MIDE.FileSystem;
using System.Reflection;
using MIDE.API.Services;
using MIDE.API.Commands;
using MIDE.API.Components;
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
        private Assembly callingAssembly;
        private readonly List<AppExtension> extensions;

        public string ApplicationName { get; private set; }
        public FileManager FileManager { get; set; }
        public IBufferProvider SystemBuffer { get; set; }
        public IEnumerable<AppExtension> Extensions => extensions;
        public IUIManager UIManager { get; set; }
        public List<IApplicationInitializer> Initializers { get; }

        public event Action ApplicationExit;

        private AppKernel ()
        {
            extensions = new List<AppExtension>();
            Initializers = new List<IApplicationInitializer>();
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
            var configs = FileManager.LoadConfigurations();
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
        public override string ToString() => $"KERNEL >> {callingAssembly?.FullName ?? "?"}";

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
            extension.Kernel = this;
            extension.Initialize();
            extensions.Add(extension);
        }
        
        /// <summary>
        /// Loads all the extensions that are provided in attached assemblies
        /// </summary>
        private void LoadExtensions()
        {
            //TODO: load extensions DLLs
            //Assembly assembly = Assembly.LoadFrom("file");
            //var types = assembly.GetTypes();
            //for (int i = 0; i < types.Length; i++)
            //{
            //    bool isExtension = types[i].IsSubclassOf(typeof(AppExtension));
            //    if (isExtension)
            //    {
            //        //TODO: add extension
            //    }
            //}
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

        public void Dispose()
        {
            if (isRunning)
                throw new InvalidOperationException("Can not dispose application resources while it's running");
            //TODO: dispose all the application resources
            UnloadExtensions();
        }
    }
}