using System;
using System.Linq;
using System.Collections.Generic;

using Vardirsoft.XApp.IoC;
using Vardirsoft.XApp.Logging;
using Vardirsoft.XApp.Components;
using Vardirsoft.XApp.Application;
using Vardirsoft.XApp.Helpers;

namespace Vardirsoft.XApp.Extensibility
{
    public abstract class AppExtension : ApplicationComponent, IDisposable
    {
        private readonly List<Module> _modules;

        public bool IsEnabled { get; }
        
        public bool IsInitialized { get; private set; }
        
        public IEnumerable<Module> Modules => _modules;

        public AppExtension(string id, bool isEnabled) : base(id)
        {
            IsEnabled = isEnabled;
            _modules = new List<Module>();
        }

        public void Initialize()
        {
            if (IsEnabled)
            {
                var logger = IoCContainer.Resolve<ILogger>();
                logger.PushDebug(null, $"Extension {Id} :: begin initialization");

                RegisterMenuItems(IoCContainer.Resolve<UIManager>().ApplicationMenu);
                logger.PushDebug(null, $"Extension {Id} :: menu items loaded");

                RegisterModules();

                if (_modules.Count > 0)
                {   
                    logger.PushDebug(null, $"Extension {Id} :: modules loaded");
                }

                logger.PushDebug(null, $"Extension {Id} :: initialization completed");
                IsInitialized = true;
            }
        }
        public void Unload()
        {
            if (IsInitialized)
            {
                foreach (var module in _modules)
                {
                    //if (module.IsRunning)
                    //    module.Stop();
                    module.Unload();
                }

                _modules.Clear();
                Dispose();
            }
        }

        public T GetModule<T>(string id)
            where T: Module
        {
            var module = _modules.Find(m => m.Id == id);

            return module as T;
        }
        public override string ToString() => $"EXTENSION [{GetType().Name}] :: {Id}";

        protected void RegisterModule(Module module)
        {
            Guard.EnsureNotNull(module, typeof(ArgumentNullException));
            Guard.EnsureNot(_modules.Any(m => m.Id == module.Id), typeof(ArgumentNullException), "Duplicate module ID");

            var validation = Kernel.VerifyModule(module);
            
            var failure = (OperationResult.Failure)validation;
            Guard.EnsureIsNull(failure, typeof(ArgumentException), $"The given module [{module.Id}] is invalid: {failure.Message}");

            module.Extension = this;
            _modules.Add(module);
            module.Initialize();

            IoCContainer.Resolve<ILogger>().PushDebug(null, $"Extension {Id} :: module {module.Id} registered");
        }

        protected abstract void RegisterMenuItems(IMenuConstructionContext context);
        protected abstract void RegisterModules();

        public void Dispose()
        {
            foreach (var module in _modules)
            {
                //if (module.IsRunning)
                //    module.Stop();
                module.Extension = null;
                module.Dispose();
            }
            
            _modules.Clear();
        }
    }
}