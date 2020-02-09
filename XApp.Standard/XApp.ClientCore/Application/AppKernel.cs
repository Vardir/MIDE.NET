using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using Vardirsoft.Shared.Helpers;

using Vardirsoft.XApp.API;
using Vardirsoft.XApp.IoC;
using Vardirsoft.XApp.Logging;
using Vardirsoft.XApp.FileSystem;
using Vardirsoft.XApp.Application.Tasks;
using Vardirsoft.XApp.Application.Attributes;
using Vardirsoft.XApp.Application.Configuration;
using Vardirsoft.XApp.Helpers;

using Module = Vardirsoft.XApp.Extensibility.Module;

namespace Vardirsoft.XApp.Application
{
    public class AppKernel : IDisposable
    {
        private static AppKernel _instance;
        public static AppKernel Instance => _instance ??= new AppKernel();

        private bool _isRunning;
        private Assembly _callingAssembly;
        
        private readonly ApplicationPaths _paths;
        private readonly LinkedList<AppTask> _tasks;
        private readonly Assembly currentAssembly;

        #region Public properties

        public DateTime TimeStarted { get; private set; }

        public Version KernelVersion { get; }

        public string ApplicationName { get; private set; }

        #endregion

        public event Action ApplicationExit;

        private AppKernel()
        {
            _paths = ApplicationPaths.Instance;
            _tasks = new LinkedList<AppTask>();

            currentAssembly = Assembly.GetAssembly(typeof(AppKernel));
            
            var version = currentAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            KernelVersion = Version.Parse(version.InformationalVersion);

            var logger = IoCContainer.Resolve<ILogger>();
            logger.FatalEventRegistered += AppLogger_FatalEventRegistered;

            logger.PushDebug(null, "Application Kernel created");
        }

        public void Start()
        {
            var logger = IoCContainer.Resolve<ILogger>();

            logger.PushDebug(null, "Application Kernel starting");

            try
            {
                Guard.EnsureNot(_isRunning, typeof(ApplicationException), "Application kernel is already loaded and running");
                Guard.Ensure(IoCContainer.IsRegistered<IClipboardProvider>(), "The SystemBuffer expected to be assigned before application start");
                Guard.Ensure(IoCContainer.IsRegistered<UIManager>(), "The UIManager expected to be assigned before application start");
                Guard.Ensure(IoCContainer.IsRegistered<IFileManager>(), "The FileManager expected to be instantiated before application start");
                Guard.Ensure(IoCContainer.IsRegistered<ConfigurationManager>(), "The ConfigurationManager expected to be instantiated before application start");

                _callingAssembly = Assembly.GetCallingAssembly();

                var assemblyVerification = VerifyAssemblyAttributes();

                Guard.EnsureEmpty(assemblyVerification, typeof(ApplicationException), assemblyVerification);
            }
            catch (Exception ex)
            {
                _isRunning = true;
                logger.PushFatal(ex.Message);
            }

            LoadConfigurations();

            var langPath = IoCContainer.Resolve<IFileManager>().Combine(_paths[ApplicationPaths.ASSETS], "lang", $"{IoCContainer.Resolve<ConfigurationManager>()["lang"]}.json");
            
            IoCContainer.Resolve<ILocalizationProvider>().LoadFrom(langPath);
            
            LoadTasks();
            OnStarting();

            logger.PushDebug(null, "Application Kernel started");
            TimeStarted = DateTime.UtcNow;
            _isRunning = true;

            try
            {
                IoCContainer.Resolve<ExtensionsManager>().LoadExtensions();
            }
            catch (Exception ex)
            {
                logger.PushError(ex, this);
            }

            OnStarted();
        }

        public void SaveLog()
        {
            var logger = IoCContainer.Resolve<ILogger>();

            if (logger.EventsCount != 0)
            {
                var folder = $"{_paths[ApplicationPaths.LOGS]}\\{TimeStarted:dd-M-yyyy HH-mm-ss}\\";

                IoCContainer.Resolve<IFileManager>().MakeFolder(folder);
                logger.SaveToFile(folder, "log.txt", info: new[] { ApplicationName, KernelVersion.ToString() });
            }
        }

        /// <summary>
        /// Stops all the current threads, releases all resources and closes the application kernel
        /// </summary>
        /// <exception cref="ApplicationException"></exception>
        public void Exit()
        {
            var logger = IoCContainer.Resolve<ILogger>();
            if (_isRunning)
            {
                _isRunning = false;
                logger.PushDebug(null, "Application Kernel stopped");

                Dispose();
                logger.PushDebug(null, "Application Kernel resources are disposed");

                SaveLog();
                SaveTasks();

                IoCContainer.Resolve<ConfigurationManager>().SaveTo("config.json");

                OnExit();

                ApplicationExit?.Invoke();
            }
            else
            {
                logger.PushWarning("Attempt to terminate application while it is not started yet");
            }
        }
        public void AddTask(AppTask task)
        {
            if (task.HasValue())
            {
                if (_tasks.Contains(task))
                    return;

                _tasks.AddLast(task);
            }
        }

        public OperationResult VerifyModule(Module module)
        {
            if (module is null)
                return OperationResult.FailWith("Null module reference");

            return OperationResult.SuccessfulResult();
        }
        
        public override string ToString() => $"KERNEL [{KernelVersion}] >> {_callingAssembly?.FullName ?? "?"}";

        public void Dispose()
        {
            if (_isRunning)
            {
                IoCContainer.Resolve<ILogger>().PushWarning("Attempt to dispose of resources while application still running");
                
                return;
            }

            IoCContainer.Resolve<ExtensionsManager>().Dispose();
        }

        private void LoadConfigurations()
        {
            var logger = IoCContainer.Resolve<ILogger>();
            logger.PushDebug(null, "Loading application configurations");
            
            var configuration = IoCContainer.Resolve<ConfigurationManager>();

            try
            {
                configuration.LoadFrom("config.json");

                var value = configuration["XAPP.Kernel"];
                var parsed = Version.TryParse(value, out Version version);

                if (!parsed || version != KernelVersion)
                    configuration.AddOrUpdate(new Config("XAPP.Kernel", KernelVersion.ToString(3)));

                var loggingLevel = LoggingLevel.NONE;
                var arr = configuration["log_levels"].Trim().Split(',');

                if (arr.HasItems())
                {
                    var type = typeof(LoggingLevel);
                    loggingLevel = arr.Aggregate(loggingLevel, (accum, level) => accum | (LoggingLevel)Enum.Parse(type, level.ToString(), true));
                }

                logger.SetLoggingLevel(this, loggingLevel);
            }
            catch (Exception ex)
            {
                logger.PushFatal(ex.Message);
            }
            
            logger.FilterEvents(logger.Levels);
            IoCContainer.Resolve<AssetsManager>().LoadAssets(_paths[ApplicationPaths.ASSETS]);

            logger.PushDebug(null, "Application configurations loaded");
        }
       
        private void SaveTasks()
        {
            var fileManager = IoCContainer.Resolve<IFileManager>();
            var i = 0;

            foreach (var task in _tasks)
            {
                if (task.RepetitionMode == TaskRepetitionMode.NotLimitedOnce && task.Origin.HasValue())
                {    
                    fileManager.Delete(task.Origin);
                }

                if (task.Origin.HasValue())
                    continue;

                var path = fileManager.Combine(_paths[ApplicationPaths.TASKS], task.ToString() + i + ".bin");
                
                fileManager.Serialize(task, path);
                i++;
            }
        }
        private void LoadTasks()
        {
            var fileManager = IoCContainer.Resolve<IFileManager>();
            var files = fileManager.EnumerateFiles(_paths[ApplicationPaths.TASKS], "*.bin");

            foreach (var file in files)
            {
                var task = fileManager.Deserialize<AppTask>(file);
                task.Origin = file;
                _tasks.AddLast(task);
            }
        }
        
        private void OnStarting()
        {
            ApplyTaskAction(TaskActivationEvent.BeforeLoad);
            _tasks.Remove(at => at.ActivationEvent == TaskActivationEvent.BeforeLoad && at.RepetitionMode == TaskRepetitionMode.Once);
        }
        
        private void OnStarted()
        {
            ApplyTaskAction(TaskActivationEvent.OnLoad);
            _tasks.Remove(at => at.ActivationEvent == TaskActivationEvent.OnLoad && at.RepetitionMode == TaskRepetitionMode.Once);
        }
        
        private void OnExit()
        {
            ApplyTaskAction(TaskActivationEvent.OnExit);
            _tasks.Remove(at => at.ActivationEvent == TaskActivationEvent.OnExit && at.RepetitionMode == TaskRepetitionMode.Once);
        }
        
        private void ApplyTaskAction(TaskActivationEvent activation)
        {
            var fileManager = IoCContainer.Resolve<IFileManager>();
            var path = _paths[ApplicationPaths.TASKS];

            _tasks.ForEach(task =>
            {
                if (task.ActivationEvent == activation)
                {
                    task.Run();

                    if (task.RepetitionMode == TaskRepetitionMode.Once)
                    {    
                        fileManager.Delete(fileManager.Combine(path, task.Origin));
                    }
                }
            });
        }

        private string VerifyAssemblyAttributes()
        {
            var logger = IoCContainer.Resolve<ILogger>();
            logger.PushDebug(null, "Verifying assembly attributes");

            var hasAppPropsAttribute = false;
            var attributes = Attribute.GetCustomAttributes(_callingAssembly);

            foreach (var attribute in attributes)
            {
                switch (attribute)
                {
                    case ApplicationPropertiesAttribute attr:
                        hasAppPropsAttribute = true;
                        ApplicationName = attr.ApplicationName;
                        break;
                }
            }

            if (hasAppPropsAttribute)
            {
                logger.PushDebug(null, "Assembly attributes verified");

                return null;
            }

            return "Missing application properties attribute in core assembly file";
        }

        private void AppLogger_FatalEventRegistered(object sender, FatalEvent e)
        {
            IoCContainer.Resolve<ILogger>().PushInfo("Closing application due to fatal error");
            
            Exit();
        }
    }
}