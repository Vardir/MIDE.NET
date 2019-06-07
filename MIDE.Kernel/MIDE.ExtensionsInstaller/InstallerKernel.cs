using NuGet;
using System;
using MIDE.FileSystem;
using Newtonsoft.Json;
using MIDE.Schemes.JSON;
using MIDE.Application.Logging;
using MIDE.Application.Configuration;
using MIDE.ExtensionsInstaller.Components;

namespace MIDE.ExtensionsInstaller
{
    public class InstallerKernel
    {
        private static InstallerKernel instance;
        public static InstallerKernel Instance => instance ?? (instance = new InstallerKernel());

        private ApplicationPaths paths;
        private Installer installer;
        private Uninstaller uninstaller;
        private DefaultPackagePathResolver localPathResolver;

        public DateTime TimeStarted { get; private set; }
        public Logger EventLogger { get; }

        public event Action KernelStopped;

        private InstallerKernel()
        {
            TimeStarted = DateTime.UtcNow;
            paths = ApplicationPaths.Instance;
            EventLogger = new Logger(LoggingLevel.ALL, true);
            EventLogger.FatalEventRegistered += EventLogger_FatalEventRegistered;
            installer = new Installer(EventLogger);
            uninstaller = new Uninstaller(EventLogger);
        }
        
        public void Execute()
        {
            LoadConfigurations();
            FetchActions();
            ApplyActions();
        }
        public void Exit()
        {
            FileManager.Delete(paths["installer-config"]);
            EventLogger.PushInfo("Kernel stopped");
            EventLogger.PushInfo("Closing application");
            SaveLog();
            KernelStopped?.Invoke();
        }

        private void LoadConfigurations()
        {
            EventLogger.PushInfo("Loading configurations");
            try
            {
                ConfigurationManager.Instance.LoadFrom("config.json");
            }
            catch (Exception ex)
            {
                EventLogger.PushFatal(ex.Message);
            }
            string path = paths[ApplicationPaths.EXTENSIONS];
            path = FileManager.GetAbsolutePath(path);
            paths.AddOrUpdate(ApplicationPaths.EXTENSIONS, path);
            paths.AddOrUpdate("installer-config", FileManager.Combine(path, "installer.json"));
            paths.AddOrUpdate("installer-log", FileManager.Combine(path, "installer"));
            if (!FileManager.DirectoryExists(paths["installer-log"]))
                FileManager.MakeFolder(paths["installer-log"]);
            else
                FileManager.CleanDirectory(paths["installer-log"]);
            localPathResolver = new DefaultPackagePathResolver(paths[ApplicationPaths.EXTENSIONS]);
            installer.LocalPathResolver = localPathResolver;
            EventLogger.PushInfo("Configurations loaded");
        }
        private void FetchActions()
        {
            EventLogger.PushInfo("Fetching actions");
            try
            {
                string data = FileManager.TryRead(paths["installer-config"]);
                if (data == null)
                    return;
                var config = JsonConvert.DeserializeObject<ExtensionsInstall[]>(data);
                if (config == null)
                    return;
                for (int i = 0; i < config.Length; i++)
                {
                    var item = config[i];
                    InstallerTabSection.Instance.AddInstallationAction(item.Id, item.Repository, item.Mode, item.AutoEnable);
                }
            }
            catch (Exception ex)
            {
                EventLogger.PushError(ex, null);
            }
            EventLogger.PushInfo("Actions fetched");
        }
        private void ApplyActions()
        {
            var actions = InstallerTabSection.Instance.GetInstallationActions();
            foreach (var viewModel in actions)
            {
                string message = null;
                switch (viewModel.InstallationMode)
                {
                    case InstallationMode.Install:
                        message = installer.Install(viewModel); break;
                    case InstallationMode.Uninstall:
                        message = uninstaller.Uninstall(viewModel); break;
                }
                if (message != null)
                    EventLogger.PushWarning(message);
            }
        }
        private void SaveLog()
        {
            if (EventLogger.EventsCount == 0)
                return;
            EventLogger.SaveToFile(paths["installer-log"], "log.txt", 
                                   info: new[] { "Extensions installer", $"Installation finished at {DateTime.UtcNow}" });
        }
        
        private void EventLogger_FatalEventRegistered(object sender, FatalEvent e)
        {
            EventLogger.PushWarning("Closing application due to fatal error");
            Exit();
        }
    }
}