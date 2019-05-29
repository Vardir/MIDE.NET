using System;
using System.Linq;
using MIDE.Helpers;
using Newtonsoft.Json;
using MIDE.FileSystem;
using System.Reflection;
using MIDE.API.Services;
using MIDE.Schemes.JSON;
using MIDE.Application.Tasks;
using MIDE.Application.Events;
using MIDE.Application.Logging;
using System.Collections.Generic;
using MIDE.Application.Attributes;
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
        private FileManager fileManager;
        private Assembly currentAssembly;
        private Assembly callingAssembly;
        private ApplicationPaths paths;
        private LinkedList<AppTask> tasks;
        private ConfigurationManager configuration;

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
        /// <summary>
        /// Application-wide logger
        /// </summary>
        public Logger AppLogger { get; }
        /// <summary>
        /// Application UI manager
        /// </summary>
        public UIManager UIManager { get; set; }
        /// <summary>
        /// Application-wide event broadcaster to provide event-based interaction between application components
        /// </summary>
        public EventBroadcaster Broadcaster { get; }
        /// <summary>
        /// System clipboard interface that helps interact between application and system
        /// </summary>
        public IClipboardProvider SystemClipboard { get; set; }
        /// <summary>
        /// List of application initializers used to configure kernel and it's parts
        /// </summary>
        public List<IApplicationInitializer> Initializers { get; } 
        #endregion

        public event Action ApplicationExit;

        private AppKernel()
        {
            paths = ApplicationPaths.Instance;
            fileManager = FileManager.Instance;
            configuration = ConfigurationManager.Instance;
            tasks = new LinkedList<AppTask>();
            Initializers = new List<IApplicationInitializer>();

            currentAssembly = Assembly.GetAssembly(typeof(AppKernel));
            var version = currentAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            KernelVersion = Version.Parse(version.InformationalVersion);

            AppLogger = new Logger(LoggingLevel.ALL, useUtcTime: true);
            AppLogger.FatalEventRegistered += AppLogger_FatalEventRegistered;

            AppLogger.PushDebug(null, "Application Kernel created");
        }

        /// <summary>
        /// Starts the application kernel
        /// </summary>
        public void Start()
        {
            AppLogger.PushDebug(null, "Application Kernel starting");
            try
            {
                if (isRunning)
                    throw new ApplicationException("Application kernel is already loaded and running!");
                if (SystemClipboard == null)
                    throw new NullReferenceException("The SystemBuffer expected to be assigned before application start");
                if (UIManager == null)
                    throw new NullReferenceException("The UIHandler expected to be assigned before application start");
                callingAssembly = Assembly.GetCallingAssembly();
                string assemblyVertification = VerifyAssemblyAttributes();
                if (assemblyVertification != null)
                    throw new ApplicationException(assemblyVertification);
            }
            catch (Exception ex)
            {
                isRunning = true;
                AppLogger.PushFatal(ex.Message);
            }
            LoadConfigurations();
            LoadTasks();
            OnStarting();
            AppLogger.PushDebug(null, "Application Kernel started");
            TimeStarted = DateTime.UtcNow;
            isRunning = true;
            foreach (var initializer in Initializers)
            {
                initializer.Execute(this);
            }
            try
            {
                ExtensionsManager.Instance.LoadExtensions();
            }
            catch (Exception ex)
            {
                AppLogger.PushError(ex, this);
            }
            OnStarted();
        }
        /// <summary>
        /// Saves current session's log entries
        /// </summary>
        public void SaveLog()
        {
            if (AppLogger.EventsCount == 0)
                return;
            string folder = $"{paths[ApplicationPaths.LOGS]}\\{TimeStarted.ToString("dd-M-yyyy HH-mm-ss")}\\";
            fileManager.MakeFolder(folder);
            AppLogger.SaveToFile(folder, "log.txt", info: new[] { ApplicationName, KernelVersion.ToString() });
        }
        /// <summary>
        /// Stops all the current threads, releases all resources and closes the application kernel
        /// </summary>
        /// <exception cref="ApplicationException"></exception>
        public void Exit()
        {
            if (!isRunning)
            {
                AppLogger.PushWarning("Attempt to terminate application while it is not started yet");
                return;
            }
            AppLogger.PushDebug(null, "Application Kernel stopped");
            isRunning = false;
            ClearTemporaryFiles();
            Dispose();
            AppLogger.PushDebug(null, "Application Kernel resources are disposed");
            SaveLog();
            SaveTasks();

            configuration.SaveTo("config.json");
            OnExit();
            ApplicationExit?.Invoke();
        }
        public void AddTask(AppTask task)
        {
            if (task == null)
                return;
            if (tasks.Contains(task))
                return;
            tasks.AddLast(task);
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
                AppLogger.PushWarning("Attempt to dispose of resources while application still running");
                return;
            }
            //TODO: dispose all the application resources
            ExtensionsManager.Instance.Dispose();
        }

        private void LoadConfigurations()
        {
            AppLogger.PushDebug(null, "Loading application configurations");
            try
            {
                configuration.LoadFrom("config.json");
                string value = configuration["MIDE.Kernel"] as string;
                bool parsed = Version.TryParse(value, out Version version);
                if (!parsed || version != KernelVersion)
                    configuration.AddOrUpdate(new Config("MIDE.Kernel", KernelVersion.ToString(3)));

                LoggingLevel loggingLevel = LoggingLevel.NONE;
                string[] arr = configuration["log_levels"].Trim().Split(',');
                if (arr != null)
                {
                    Type type = typeof(LoggingLevel);
                    for (int i = 0; i < arr.Length; i++)
                    {
                        loggingLevel |= (LoggingLevel)Enum.Parse(type, arr[i]?.ToString(), true);
                    }
                }
            }
            catch (Exception ex)
            {
                AppLogger.PushFatal(ex.Message);
            }
            
            AppLogger.FilterEvents(AppLogger.Levels);
            paths.LoadFrom("paths.json");
            AssetManager.Instance.LoadAssets(paths[ApplicationPaths.ASSETS]);
            AppLogger.PushDebug(null, "Application configurations loaded");
        }
       
        private void ClearTemporaryFiles()
        {
            AppLogger.PushDebug(null, "Clearing temporary files");
            FileManager.Instance.CleanDirectory(paths[ApplicationPaths.TEMP]);
            AppLogger.PushDebug(null, "Temporary files cleared");
        }
        private void SaveTasks()
        {
            int i = 0;
            foreach (var task in tasks)
            {
                if (task.RepetitionMode == TaskRepetitionMode.NotLimitedOnce && task.Origin != null)
                    fileManager.Delete(task.Origin);
                if (task.Origin != null)
                    continue;
                string path = fileManager.Combine(paths[ApplicationPaths.TASKS], task.ToString() + i + ".bin");
                fileManager.Serialize(task, path);
                i++;
            }
        }
        private void LoadTasks()
        {
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
            tasks.Remove(at => at.ActivationEvent == TaskActivationEvent.BeforeLoad && 
                               at.RepetitionMode == TaskRepetitionMode.Once);
        }
        private void OnStarted()
        {
            ApplyTaskAction(TaskActivationEvent.OnLoad);
            tasks.Remove(at => at.ActivationEvent == TaskActivationEvent.OnLoad &&
                               at.RepetitionMode == TaskRepetitionMode.Once);
        }
        private void OnExit()
        {
            ApplyTaskAction(TaskActivationEvent.OnExit);
            tasks.Remove(at => at.ActivationEvent == TaskActivationEvent.OnExit &&
                               at.RepetitionMode == TaskRepetitionMode.Once);
        }
        private void ApplyTaskAction(TaskActivationEvent activation)
        {
            var path = paths[ApplicationPaths.TASKS];
            tasks.ForEach(at =>
            {
                if (at.ActivationEvent != activation)
                    return;
                at.Run();
                if (at.RepetitionMode == TaskRepetitionMode.Once)
                    fileManager.Delete(fileManager.Combine(path, at.Origin));
            });
        }

        private string VerifyAssemblyAttributes()
        {
            AppLogger.PushDebug(null, "Verifying assembly attributes");
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
                return "Missing application properties attribute in core assembly file";
            AppLogger.PushDebug(null, "Assembly attributes verified");

            return null;
        }

        private void AppLogger_FatalEventRegistered(object sender, FatalEvent e)
        {
            AppLogger.PushInfo("Closing application due to fatal error");
            Exit();
        }
    }
}