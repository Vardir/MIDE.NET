using System;
using MIDE.IoC;
using System.Linq;
using MIDE.Helpers;
using MIDE.Components;
using MIDE.Application;
using System.Collections.Generic;

namespace MIDE.Extensibility
{
    public abstract class AppExtension : ApplicationComponent, IDisposable
    {
        private readonly List<Module> modules;

        /// <summary>
        /// A flag to indicate whether the extension is enabled to load
        /// </summary>
        public bool IsEnabled { get; }
        /// <summary>
        /// A flag to indicate whether the extension was initialized
        /// </summary>
        public bool IsInitialized { get; private set; }
        
        public IEnumerable<Module> Modules => modules;

        public AppExtension(string id, bool isEnabled) : base(id)
        {
            IsEnabled = isEnabled;
            modules = new List<Module>();
        }

        public void Initialize()
        {
            if (IsEnabled)
            {
                Kernel.AppLogger.PushDebug(null, $"Extension {Id} :: begin initialization");

                RegisterMenuItems(IoCContainer.Resolve<UIManager>().ApplicationMenu);
                Kernel.AppLogger.PushDebug(null, $"Extension {Id} :: menu items loaded");

                RegisterModules();

                if (modules.Count > 0)
                    Kernel.AppLogger.PushDebug(null, $"Extension {Id} :: modules loaded");

                Kernel.AppLogger.PushDebug(null, $"Extension {Id} :: initialization completed");
                IsInitialized = true;
            }
        }
        public void Unload()
        {
            if (IsInitialized)
            {
                foreach (var module in modules)
                {
                    //if (module.IsRunning)
                    //    module.Stop();
                    module.Unload();
                }
                modules.Clear();
                Dispose();
            }
        }

        public T GetModule<T>(string id)
            where T: Module
        {
            var module = modules.Find(m => m.Id == id);
            if (module == null)
                return null;
                
            return module as T;
        }
        public override string ToString() => $"EXTENSION [{GetType().Name}] :: {Id}";

        /// <summary>
        /// Registers the module in the internal extension storage
        /// </summary>
        /// <param name="module"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        protected void RegisterModule(Module module)
        {
            if (module == null)
                throw new ArgumentNullException("Module parameter can not be null");

            if (modules.Any(m => m.Id == module.Id))
                throw new ArgumentException("Duplicate module ID");

            var validation = Kernel.VerifyModule(module);
            if (validation.HasValue())
                throw new ArgumentException($"The given module [{module.Id}] is invalid: {validation}");

            module.Extension = this;
            modules.Add(module);
            module.Initialize();

            Kernel.AppLogger.PushDebug(null, $"Extension {Id} :: module {module.Id} registered");
        }

        protected abstract void RegisterMenuItems(IMenuConstructionContext context);
        protected abstract void RegisterModules();

        public void Dispose()
        {
            foreach (var module in modules)
            {
                //if (module.IsRunning)
                //    module.Stop();
                module.Extension = null;
                module.Dispose();
            }
            modules.Clear();
        }
    }
}