//using NuGet;
using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Vardirsoft.Shared.Helpers;
using Vardirsoft.XApp.IoC;
using Vardirsoft.XApp.API;
using Vardirsoft.XApp.Logging;
using Vardirsoft.XApp.FileSystem;
using Vardirsoft.XApp.Schemes.JSON;
using Vardirsoft.XApp.Application.Configuration;
using Vardirsoft.XApp.ExtensionsInstaller.ViewModels;

namespace Vardirsoft.XApp.ExtensionsInstaller
{
    public class InstallerKernel
    {
        private static InstallerKernel _instance;
        public static InstallerKernel Instance => _instance ??= new InstallerKernel();

        private readonly ApplicationPaths _paths;
        private readonly Installer _installer;
        private readonly Uninstaller _uninstaller;
        //private DefaultPackagePathResolver localPathResolver;

        public DateTime TimeStarted { get; private set; }
        public Logger EventLogger { get; }

        public event Action KernelStopped;

        private InstallerKernel()
        {
            TimeStarted = DateTime.UtcNow;
            _paths = ApplicationPaths.Instance;
            EventLogger = new Logger(LoggingLevel.ALL, true);
            EventLogger.FatalEventRegistered += EventLogger_FatalEventRegistered;
            _installer = new Installer(EventLogger);
            _uninstaller = new Uninstaller(EventLogger);
        }
        
        public void Execute()
        {
            LoadConfigurations();
            FetchActions();
            ApplyActions();
        }
        public void Exit()
        {
            IoCContainer.Resolve<IFileManager>().Delete(_paths["installer-config"]);
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
                IoCContainer.Resolve<ConfigurationManager>().LoadFrom("config.json");
            }
            catch (Exception ex)
            {
                EventLogger.PushFatal(ex.Message);
            }

            var path = _paths[ApplicationPaths.EXTENSIONS];
            var fileManager = IoCContainer.Resolve<IFileManager>();

            path = fileManager.GetAbsolutePath(path);
            _paths.AddOrUpdate(ApplicationPaths.EXTENSIONS, path);
            _paths.AddOrUpdate("installer-config", fileManager.Combine(path, "installer.json"));
            _paths.AddOrUpdate("installer-log", fileManager.Combine(path, "installer"));

            if (fileManager.DirectoryExists(_paths["installer-log"]))
            {
                fileManager.CleanDirectory(_paths["installer-log"]);
            }
            else
            {
                fileManager.MakeFolder(_paths["installer-log"]);
            }
            //localPathResolver = new DefaultPackagePathResolver(paths[ApplicationPaths.EXTENSIONS]);
            //installer.LocalPathResolver = localPathResolver;
            EventLogger.PushInfo("Configurations loaded");
        }
        private void FetchActions()
        {
            EventLogger.PushInfo("Fetching actions");
            try
            {
                var data = IoCContainer.Resolve<IFileManager>().TryRead(_paths["installer-config"]);
                if (data is null)
                    return;

                var config = JsonConvert.DeserializeObject<ExtensionsInstall[]>(data);
                if (config is null)
                    return;

                for (var i = 0; i < config.Length; i++)
                {
                    var item = config[i];
                    //InstallerTabSection.Instance.AddInstallationAction(item.Id, item.Repository, item.Mode, item.AutoEnable);
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
            IEnumerable<InstallationActionViewModel> actions = null;//InstallerTabSection.Instance.GetInstallationActions();
            foreach (var viewModel in actions)
            {
                string message = null;
                switch (viewModel.InstallationMode)
                {
                    case InstallationMode.Install:
                        message = _installer.Install(viewModel); break;
                    case InstallationMode.Uninstall:
                        message = _uninstaller.Uninstall(viewModel); break;
                }
                
                if (message.HasValue())
                {    
                    EventLogger.PushWarning(message);
                }
            }
        }
        private void SaveLog()
        {
            if (EventLogger.EventsCount == 0)
                return;
                
            EventLogger.SaveToFile(_paths["installer-log"], "log.txt", 
                                   info: new[] { "Extensions installer", $"Installation finished at {DateTime.UtcNow}" });
        }
        
        private void EventLogger_FatalEventRegistered(object sender, FatalEvent e)
        {
            EventLogger.PushWarning("Closing application due to fatal error");
            Exit();
        }
    }
}