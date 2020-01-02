using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using Vardirsoft.Shared.Helpers;

using Vardirsoft.XApp.API;
using Vardirsoft.XApp.IoC;
using Vardirsoft.XApp.Logging;
using Vardirsoft.XApp.FileSystem;
using Vardirsoft.XApp.Application.Tasks;
using Vardirsoft.XApp.Application.Attributes;
using Vardirsoft.XApp.Application.Configuration;
using Module = Vardirsoft.XApp.Extensibility.Module;

namespace Vardirsoft.XApp.Application
{
    public class AppKernel : IDisposable
    {
        private static AppKernel instance;
        public static AppKernel Instance => instance ?? (instance = new AppKernel());

        private bool isRunning;
        private Assembly currentAssembly;
        private Assembly callingAssembly;
        private ApplicationPaths paths;
        private LinkedList<AppTask> tasks;

        #region Public properties

        /// <summary>
        /// A time when application kernel was started
        /// </summary>
        public DateTime TimeStarted { get; private set; }

        /// <summary>
        /// Version of application kernel
        /// </summary>
        public Version KernelVersion { get; }

        /// <summary>
        /// Name of application kernel
        /// </summary>
        public string ApplicationName { get; private set; }

        #endregion

        public event Action ApplicationExit;

        private AppKernel()
        {
            paths = ApplicationPaths.Instance;
            tasks = new LinkedList<AppTask>();

            currentAssembly = Assembly.GetAssembly(typeof(AppKernel));
            
            var version = currentAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            KernelVersion = Version.Parse(version.InformationalVersion);

            var logger = IoCContainer.Resolve<ILogger>();
            logger.FatalEventRegistered += AppLogger_FatalEventRegistered;

            logger.PushDebug(null, "Application Kernel created");
        }

        /// <summary>
        /// Starts the application kernel
        /// </summary>
        public void Start()
        {
            var logger = IoCContainer.Resolve<ILogger>();

            logger.PushDebug(null, "Application Kernel starting");

            try
            {
                if (isRunning)
                    throw new ApplicationException("Application kernel is already loaded and running!");

                if (IoCContainer.Resolve<IClipboardProvider>() == null)
                    throw new NullReferenceException("The SystemBuffer expected to be assigned before application start");

                if (IoCContainer.Resolve<UIManager>() == null)
                    throw new NullReferenceException("The UIManager expected to be assigned before application start");

                if (IoCContainer.Resolve<IFileManager>() == null)
                    throw new NullReferenceException("The FileManager expected to be instantiated before application start");
                
                if (IoCContainer.Resolve<ConfigurationManager>() == null)
                    throw new NullReferenceException("The ConfigurationManager expected to be instantiated before application start");

                callingAssembly = Assembly.GetCallingAssembly();

                var assemblyVerification = VerifyAssemblyAttributes();

                if (assemblyVerification.HasValue())
                    throw new ApplicationException(assemblyVerification);
            }
            catch (Exception ex)
            {
                isRunning = true;
                logger.PushFatal(ex.Message);
            }

            LoadConfigurations();

            var langPath = IoCContainer.Resolve<IFileManager>().Combine(paths[ApplicationPaths.ASSETS], 
                                                                        "lang", 
                                                                        $"{IoCContainer.Resolve<ConfigurationManager>()["lang"]}.json");
            
            IoCContainer.Resolve<ILocalizationProvider>().LoadFrom(langPath);
            
            LoadTasks();
            OnStarting();

            logger.PushDebug(null, "Application Kernel started");
            TimeStarted = DateTime.UtcNow;
            isRunning = true;

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

        /// <summary>
        /// Saves current session's log entries
        /// </summary>
        public void SaveLog()
        {
            var logger = IoCContainer.Resolve<ILogger>();

            if (logger.EventsCount != 0)
            {
                var folder = $"{paths[ApplicationPaths.LOGS]}\\{TimeStarted.ToString("dd-M-yyyy HH-mm-ss")}\\";

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
            if (isRunning)
            {
                isRunning = false;
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
                if (tasks.Contains(task))
                    return;

                tasks.AddLast(task);
            }
        }

        /// <summary>
        /// Verifies if the module is valid for the current application.
        /// Returns null if valid, otherwise returns message.
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public string VerifyModule(Module module)
        {
            if (module == null)
                return "Null module reference";

            return null;
        }
        public override string ToString() => $"KERNEL [{KernelVersion}] >> {callingAssembly?.FullName ?? "?"}";

        public void Dispose()
        {
            if (isRunning)
            {
                IoCContainer.Resolve<ILogger>().PushWarning("Attempt to dispose of resources while application still running");
                
                return;
            }

            //TODO: dispose all the application resources
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

                var value = configuration["XAPP.Kernel"] as string;
                var parsed = Version.TryParse(value, out Version version);

                if (!parsed || version != KernelVersion)
                    configuration.AddOrUpdate(new Config("XAPP.Kernel", KernelVersion.ToString(3)));

                var loggingLevel = LoggingLevel.NONE;
                var arr = configuration["log_levels"].Trim().Split(',');

                if (arr.HasItems())
                {
                    var type = typeof(LoggingLevel);
                    for (int i = 0; i < arr.Length; i++)
                    {
                        loggingLevel |= (LoggingLevel)Enum.Parse(type, arr[i]?.ToString(), true);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.PushFatal(ex.Message);
            }
            
            logger.FilterEvents(logger.Levels);
            IoCContainer.Resolve<AssetsManager>().LoadAssets(paths[ApplicationPaths.ASSETS]);

            logger.PushDebug(null, "Application configurations loaded");
        }
       
        private void SaveTasks()
        {
            var fileManager = IoCContainer.Resolve<IFileManager>();
            int i = 0;

            foreach (var task in tasks)
            {
                if (task.RepetitionMode == TaskRepetitionMode.NotLimitedOnce && task.Origin.HasValue())
                {    
                    fileManager.Delete(task.Origin);
                }

                if (task.Origin.HasValue())
                    continue;

                var path = fileManager.Combine(paths[ApplicationPaths.TASKS], task.ToString() + i + ".bin");
                
                fileManager.Serialize(task, path);
                i++;
            }
        }
        private void LoadTasks()
        {
            var fileManager = IoCContainer.Resolve<IFileManager>();
            var files = fileManager.EnumerateFiles(paths[ApplicationPaths.TASKS], "*.bin");

            foreach (var file in files)
            {
                var task = fileManager.Deserialize<AppTask>(file);
                task.Origin = file;
                tasks.AddLast(task);
            }
        }
        private void OnStarting()
        {
            ApplyTaskAction(TaskActivationEvent.BeforeLoad);
            tasks.Remove(at => at.ActivationEvent == TaskActivationEvent.BeforeLoad && at.RepetitionMode == TaskRepetitionMode.Once);
        }
        private void OnStarted()
        {
            ApplyTaskAction(TaskActivationEvent.OnLoad);
            tasks.Remove(at => at.ActivationEvent == TaskActivationEvent.OnLoad && at.RepetitionMode == TaskRepetitionMode.Once);
        }
        private void OnExit()
        {
            ApplyTaskAction(TaskActivationEvent.OnExit);
            tasks.Remove(at => at.ActivationEvent == TaskActivationEvent.OnExit && at.RepetitionMode == TaskRepetitionMode.Once);
        }
        private void ApplyTaskAction(TaskActivationEvent activation)
        {
            var fileManager = IoCContainer.Resolve<IFileManager>();
            var path = paths[ApplicationPaths.TASKS];

            tasks.ForEach(task =>
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

            var hasAppPropsAttriburte = false;
            var attributes = Attribute.GetCustomAttributes(callingAssembly);

            foreach (var attribute in attributes)
            {
                switch (attribute)
                {
                    case ApplicationPropertiesAttribute attr:
                        hasAppPropsAttriburte = true;
                        ApplicationName = attr.ApplicationName;
                        break;
                }
            }

            if (hasAppPropsAttriburte)
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