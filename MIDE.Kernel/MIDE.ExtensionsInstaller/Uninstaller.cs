using NuGet;
using System;
using MIDE.FileSystem;
using MIDE.Application.Logging;
using MIDE.ExtensionsInstaller.ViewModels;

namespace MIDE.ExtensionsInstaller
{
    public sealed class Uninstaller
    {
        private Logger eventLogger;
        private FileManager fileManager;
        private PackageManager packManager;
        
        public Uninstaller(Logger eventLogger)
        {
            this.eventLogger = eventLogger;
            fileManager = FileManager.Instance;
        }

        public string Uninstall(InstallationActionViewModel action)
        {
            string id = action.ExtensionId;
            action.SetProgress(InstallationProgress.Uninstalling, "(ext-installer:progr-uninstalling)");
            eventLogger.PushInfo($"Attempting to uninstall extension '{id}'");
            try
            {
                packManager.UninstallPackage(id);
            }
            catch (Exception ex)
            {
                eventLogger.PushError(ex, null, ex.Message);
                return $"Extension '{id}': uninstallation failed";
            }
            eventLogger.PushInfo($"Extension '{id}' uninstalled");
            action.SetProgress(InstallationProgress.Success, "(ext-installer:progr-success-uninstall)");
            return null;
        }
    }
}