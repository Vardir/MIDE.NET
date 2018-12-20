using System;
using System.Linq;
using System.Reflection;
using MIDE.Standard.FileSystem;
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
        public IEnumerable<AppExtension> Extensions => extensions;

        private AppKernel ()
        {
            extensions = new List<AppExtension>();
            ApplicationMenu = new Menu("app-menu");
        }

        public void SetFileManager() { }
        public void Start()
        {
            if (isRunning)
                throw new ApplicationException("Application kernel is already loaded and running!");
            if (FileManager == null)
                throw new NullReferenceException("The FileManager should be assigned before starting the application");
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
        public void LoadExtensions()
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
        public void Exit()
        {
            if (!isRunning)
                throw new InvalidOperationException("Can not exit application that is not started");
            isRunning = false;
            UnloadSolution();
            ClearTemporaryFiles();
            Dispose();
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
        
        private void UnloadSolution()
        {

        }
        private void UnloadExtensions()
        {
            foreach (var extension in extensions)
            {
                extension.Unload();
                extension.Kernel = null;
            }
            extensions.Clear();
        }
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