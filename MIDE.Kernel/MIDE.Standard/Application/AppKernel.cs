using System;
using System.Linq;
using System.Reflection;
using MIDE.Standard.FileSystem;
using MIDE.Standard.API.Services;
using MIDE.Standard.API.Commands;
using System.Collections.Generic;
using MIDE.Standard.API.Components;
using MIDE.Standard.API.Extensibility;
using MIDE.Standard.Application.Attrubites;
using MIDE.Standard.Application.Configuration;
using Module = MIDE.Standard.API.Extensibility.Module;

namespace MIDE.Standard.Application
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
        public Menu ApplicationMenu { get; private set; }
        public IBufferProvider SystemBuffer { get; set; }
        public IEnumerable<AppExtension> Extensions => extensions;

        public event Action ApplicationExit;

        private AppKernel ()
        {
            extensions = new List<AppExtension>();
            ApplicationMenu = new Menu("app-menu");
            PopulateMenu();
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
                throw new NullReferenceException("The FileManager should be assigned before starting the application");
            if (SystemBuffer == null)
                throw new NullReferenceException("The SystemBuffer should be assigned before starting the application");
            callingAssembly = Assembly.GetCallingAssembly();
            string assemblyVertification = VerifyAssemblyAttributes();
            if (assemblyVertification != null)
                throw new ApplicationException(assemblyVertification);

            isRunning = true;
            //TODO: prepare application
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
        /// Populates the application menu with predefined basic items
        /// </summary>
        private void PopulateMenu()
        {
            ApplicationMenu.AddItem(new MenuButton("file", -99));
            ApplicationMenu.AddItem("file", new MenuSplitter("split-exit", 98));
            ApplicationMenu.AddItem("file", new MenuButton("exit", 99)
            {
                PressCommand = new RelayCommand(Exit)
            });
            ApplicationMenu.AddItem("file", new MenuButton("new", -99));
            ApplicationMenu.AddItem("file/new", new MenuButton("file", -99));
            ApplicationMenu.AddItem("file/new", new MenuButton("folder", -98));
            ApplicationMenu.AddItem(new MenuButton("edit", -98));
            ApplicationMenu.AddItem(new MenuButton("view", -97));
            ApplicationMenu.AddItem(new MenuButton("tools", 50));
            ApplicationMenu.AddItem(new MenuButton("help", 99));
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