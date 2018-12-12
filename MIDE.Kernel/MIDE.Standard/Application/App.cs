using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using MIDE.Standard.API.Extensibility;
using Module = MIDE.Standard.API.Extensibility.Module;

namespace MIDE.Standard.Application
{
    public class App : IDisposable
    {
        private static App instance;
        public static App Instance => instance ?? (instance = new App());

        private bool isRunning;
        private readonly List<AppExtension> extensions;

        public IEnumerable<AppExtension> Extensions => extensions;

        private App ()
        {
            extensions = new List<AppExtension>();
        }

        public void Start()
        {
            isRunning = true;
            //TODO: prepare application
            LoadExtensions();
        }
        public void LoadExtensions()
        {
            //TODO: load extensions DLLs
            Assembly assembly = Assembly.LoadFrom("file");
            var types = assembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                bool isExtension = types[i].IsSubclassOf(typeof(AppExtension));
                if (isExtension)
                {
                    //TODO: add extension
                }
            }
        }
        public void Exit()
        {
            if (!isRunning)
                throw new InvalidOperationException("Can not exit application that is not started");
            isRunning = false;
            UnloadSolution();
            UnloadExtensions();
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

        protected internal void RegisterExtension(AppExtension extension)
        {
            if (extension == null)
                throw new ArgumentNullException("Extension parameter can not be null");
            if (extensions.FirstOrDefault(e => e.Id == extension.Id) != null)
                throw new ArgumentException("Duplicate extension ID");
            extension.Application = this;
            extensions.Add(extension);
        }
        
        private void UnloadSolution()
        {

        }
        private void UnloadExtensions()
        {

        }
        private void ClearTemporaryFiles()
        {

        }

        public void Dispose()
        {
            if (isRunning)
                throw new InvalidOperationException("Can not dispose application resources while it's running");
            //TODO: dispose all the application resources
            foreach (var extension in extensions)
            {
                extension.Unload();
            }
            extensions.Clear();
        }
    }
}