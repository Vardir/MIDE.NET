using System;
using System.Linq;
using MIDE.Standard.Application;
using System.Collections.Generic;
using MIDE.Standard.API.Components;

namespace MIDE.Standard.API.Extensibility
{
    public abstract class AppExtension : ApplicationComponent, IDisposable
    {
        private readonly List<Module> modules;

        public bool IsInitialized { get; private set; }
        public AppKernel Kernel { get; internal set; }
        public IEnumerable<Module> Modules => modules;

        public AppExtension(string id) : base(id)
        {
            modules = new List<Module>();
        }
        
        public void Initialize()
        {
            RegisterMenuItems(Kernel.ApplicationMenu);
        }
        public void Unload()
        {
            foreach (var module in modules)
            {
                if (module.IsRunning)
                    module.Stop();
                module.Unload();
            }
            modules.Clear();
            Dispose();
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
            if (modules.FirstOrDefault(m => m.Id == module.Id) != null)
                throw new ArgumentException("Duplicate module ID");
            var validation = Kernel.VerifyModule(module);
            if (validation != null)
                throw new ArgumentException($"The given module [{module.Id}] is invalid: {validation}");
            module.Extension = this;
            modules.Add(module);
        }

        protected abstract void RegisterMenuItems(IMenuConstructionContext context);

        public void Dispose()
        {
            foreach (var module in modules)
            {
                if (module.IsRunning)
                    module.Stop();
                module.Dispose();
                module.Extension = null;
            }
            modules.Clear();
        }
    }
}