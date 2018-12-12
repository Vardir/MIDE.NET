using System;
using System.Linq;
using MIDE.Standard.Application;
using System.Collections.Generic;

namespace MIDE.Standard.API.Extensibility
{
    public class AppExtension : IDisposable
    {
        private readonly List<Module> modules;

        public string Id { get; }
        public App Application { get; internal set; }
        public IEnumerable<Module> Modules => modules;

        public AppExtension(string id)
        {
            Id = id;
            modules = new List<Module>();
        }

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
            var validation = Application.VerifyModule(module);
            if (validation != null)
                throw new ArgumentException($"The given module [{module.Id}] is invalid: {validation}");
            module.Extension = this;
            modules.Add(module);
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

        public void Dispose()
        {
            
        }
    }
}