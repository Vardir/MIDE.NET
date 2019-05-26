using NuGet;
using System;
using System.Linq;
using MIDE.FileSystem;
using Newtonsoft.Json;
using MIDE.Schemes.JSON;
using MIDE.Application.Logging;

namespace MIDE.ExtensionsInstaller
{
    public class InstallerKernel
    {
        private static InstallerKernel instance;
        public static InstallerKernel Instance => instance ?? (instance = new InstallerKernel());

        private string extensionsPath;

        public DateTime TimeStarted { get; private set; }
        public Logger EventLogger { get; }

        public event Action KernelStopped;

        private InstallerKernel ()
        {
            TimeStarted = DateTime.UtcNow;
            EventLogger = new Logger(LoggingLevel.ALL, true, false);
            EventLogger.FatalEventRegistered += EventLogger_FatalEventRegistered;
        }
        
        public void Execute()
        {
            EventLogger.PushInfo("Loading configurations");
            ApplicationConfig appConfig = null;
            try
            {
                string configData = FileManager.Instance.ReadOrCreate("config.json", $"{{ }}");
                appConfig = JsonConvert.DeserializeObject<ApplicationConfig>(configData);
            }
            catch (Exception ex)
            {
                EventLogger.PushFatal(ex.Message);
            }
            EventLogger.PushInfo("Configurations loaded");
            string extensionsPath = appConfig.Paths.FirstOrDefault(path => path.Key == "extensions")?.Value ?? "root\\extensions\\";
            string installFile = FileManager.Instance.Combine(extensionsPath, "install.json");
            string uninstallFile = FileManager.Instance.Combine(extensionsPath, "uninstall.json");
            string installData = FileManager.Instance.TryRead(installFile);
            string uninstallData = FileManager.Instance.TryRead(uninstallFile);
            try
            {
                execute(installData, (item) => Install(item.Repository, item.Id));
                execute(uninstallData, (item) => Uninstall(item.Id));
            }
            catch (Exception ex)
            {
                EventLogger.PushError(ex, null);
            }
            EventLogger.PushInfo("Actions completed");
            SaveLog();
            Exit();

            void execute(string data, Func<ExtensionInstall, string> func)
            {
                if (data == null)
                    return;
                var config = JsonConvert.DeserializeObject<ExtensionInstall[]>(data);
                if (config == null)
                    return;
                for (int i = 0; i < config.Length; i++)
                {
                    string error = func(config[i]);
                    if (error != null)
                        EventLogger.PushWarning(error);
                }
            }
        }
        public void Exit()
        {
            EventLogger.PushInfo("Kernel stopped");
            EventLogger.PushInfo("Closing application");
            KernelStopped?.Invoke();
        }

        private void SaveLog()
        {
            if (EventLogger.EventsCount == 0)
                return;
            string folder = $"{FileManager.Instance.GetPath(FileManager.LOGS)}\\{TimeStarted.ToString("dd-M-yyyy HH-mm-ss")}\\";
            FileManager.Instance.MakeFolder(folder);
            EventLogger.SaveToFile(folder, "log.txt", info: new[] { "Extensions installer" });
        }

        private string Install(string repositoryPath, string id)
        {
            EventLogger.PushInfo($"Attempting to install extension '{id}' from '{repositoryPath}'");
            var repository = PackageRepositoryFactory.Default.CreateRepository(repositoryPath);
            if (repository == null)
                return $"Can not install extension: repository '{repositoryPath}' not found";
            var package = repository.FindPackage(id);
            if (package == null)
                return $"Can not find extension with ID '{id}' on repository '{repositoryPath}'";

            PackageManager packageManager = new PackageManager(repository, extensionsPath);
            packageManager.InstallPackage(id);
            EventLogger.PushInfo($"Extension '{id}' installed");
            return null;
        }
        private string Uninstall(string id)
        {
            EventLogger.PushInfo($"Attempting to uninstall extension '{id}'");
            PackageManager packageManager = new PackageManager(null, extensionsPath);
            packageManager.UninstallPackage(id);
            EventLogger.PushInfo($"Extension '{id}' uninstalled");
            return null;
        }

        private void EventLogger_FatalEventRegistered(object sender, FatalEvent e)
        {
            Exit();
        }
    }
}