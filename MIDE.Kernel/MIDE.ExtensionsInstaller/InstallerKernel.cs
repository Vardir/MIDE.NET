using NuGet;
using System;
using System.Linq;
using MIDE.FileSystem;
using Newtonsoft.Json;
using MIDE.Schemes.JSON;
using MIDE.Application.Logging;
using MIDE.ExtensionsInstaller.Components;
using MIDE.ExtensionsInstaller.ViewModels;

namespace MIDE.ExtensionsInstaller
{
    public class InstallerKernel
    {
        private static InstallerKernel instance;
        public static InstallerKernel Instance => instance ?? (instance = new InstallerKernel());

        private FileManager fileManager;
        private PackageManager uninstallManager;
        private PackageManager installManager;

        public DateTime TimeStarted { get; private set; }
        public Logger EventLogger { get; }

        public event Action KernelStopped;

        private InstallerKernel()
        {
            fileManager = FileManager.Instance;
            TimeStarted = DateTime.UtcNow;
            EventLogger = new Logger(LoggingLevel.ALL, true);
            EventLogger.FatalEventRegistered += EventLogger_FatalEventRegistered;
        }
        
        public void Execute()
        {
            LoadConfigurations();
            FetchActions();
            ApplyActions();
        }
        public void Exit()
        {
            fileManager.Delete(fileManager["installer-config"]);
            EventLogger.PushInfo("Kernel stopped");
            EventLogger.PushInfo("Closing application");
            SaveLog();
            KernelStopped?.Invoke();
        }

        private void LoadConfigurations()
        {
            EventLogger.PushInfo("Loading configurations");
            ApplicationConfig appConfig = null;
            try
            {
                string configData = fileManager.ReadOrCreate("config.json", $"{{ }}");
                appConfig = JsonConvert.DeserializeObject<ApplicationConfig>(configData);
            }
            catch (Exception ex)
            {
                EventLogger.PushFatal(ex.Message);
            }
            EventLogger.PushInfo("Configurations loaded");
            string path = appConfig.Paths.FirstOrDefault(path => path.Key == "extensions")?.Value ?? "root\\extensions\\";
            path = fileManager.GetAbsolutePath(path);
            fileManager.AddOrUpdate(FileManager.EXTENSIONS, path);
            fileManager.AddOrUpdate("installer-config", fileManager.Combine(path, "installer.json"));
            fileManager.AddOrUpdate("installer-log", fileManager.Combine(path, "installer"));
            if (fileManager.IsDirectory(fileManager["installer-log"]))
                fileManager.MakeFolder(fileManager["installer-log"]);
            else
                fileManager.CleanDirectory(fileManager["installer-log"]);

            var repository = PackageRepositoryFactory.Default.CreateRepository(path);
            uninstallManager = new PackageManager(repository, path);
        }
        private void FetchActions()
        {
            EventLogger.PushInfo("Fetching actions");
            try
            {
                string data = fileManager.TryRead(fileManager["installer-config"]);
                if (data == null)
                    return;
                var config = JsonConvert.DeserializeObject<ExtensionInstall[]>(data);
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
                string message = viewModel.InstallationMode switch
                {
                    InstallationMode.Install => Install(viewModel),
                    InstallationMode.Uninstall => Uninstall(viewModel),
                    _ => null
                };
                if (message != null)
                    EventLogger.PushWarning(message);
            }
        }
        private void SaveLog()
        {
            if (EventLogger.EventsCount == 0)
                return;
            EventLogger.SaveToFile(fileManager["installer-log"], "log.txt", 
                                   info: new[] { "Extensions installer", $"Installation finished at {DateTime.UtcNow}" });
        }

        private string Install(InstallationActionViewModel action)
        {
            string id = action.ExtensionId;
            string repositoryPath = action.ExtensionSource;
            action.SetProgress(InstallationProgress.Loading, "Loading extension from repository");
            EventLogger.PushInfo($"Attempting to install extension '{id}' from '{repositoryPath}'");
            var packageInstaller = GetInstallerManager(repositoryPath);
            if (packageInstaller == null)
            {
                action.SetProgress(InstallationProgress.Error, "Repository not found or it is corrupted");
                return $"Can not install extension: repository '{repositoryPath}' not found";
            }
            var package = packageInstaller.SourceRepository.FindPackage(id);
            if (package == null)
            {
                action.SetProgress(InstallationProgress.Error, "Extension not found");
                return $"Can not find extension with ID '{id}' on repository '{repositoryPath}'";
            }

            action.SetProgress(InstallationProgress.Installing, "Installing extension");
            try
            {
                packageInstaller.InstallPackage(id);
            }
            catch (Exception ex)
            {
                action.SetProgress(InstallationProgress.Error, "Extension installation failed, see log");
                EventLogger.PushError(ex, null);
                return $"Extension '{id}': installation failed";
            }
            action.SetProgress(InstallationProgress.Success, "Extension installed");
            EventLogger.PushInfo($"Extension '{id}' installed");
            return null;
        }
        private string Uninstall(InstallationActionViewModel action)
        {
            string id = action.ExtensionId;
            action.SetProgress(InstallationProgress.Uninstalling, "Uninstalling");
            EventLogger.PushInfo($"Attempting to uninstall extension '{id}'");
            try
            {
                uninstallManager.UninstallPackage(id);
            }
            catch (Exception ex)
            {
                EventLogger.PushError(ex, null, ex.Message);
                return $"Extension '{id}': uninstallation failed";
            }
            EventLogger.PushInfo($"Extension '{id}' uninstalled");
            action.SetProgress(InstallationProgress.Success, "Extension uninstalled");
            return null;
        }

        private void EventLogger_FatalEventRegistered(object sender, FatalEvent e)
        {
            EventLogger.PushWarning("Closing application due to fatal error");
            Exit();
        }

        private PackageManager GetInstallerManager(string path)
        {
            if (installManager == null || installManager.SourceRepository.Source != path)
            {
                var repository = PackageRepositoryFactory.Default.CreateRepository(path);
                if (repository == null)
                    return null;
                return (installManager = new PackageManager(repository, fileManager[FileManager.EXTENSIONS]));
            }
            return installManager;
        }
    }
}