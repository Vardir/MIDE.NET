//using NuGet;
using System;

using Vardirsoft.Shared.Helpers;

using Vardirsoft.XApp.Logging;
using Vardirsoft.XApp.ExtensionsInstaller.ViewModels;

namespace Vardirsoft.XApp.ExtensionsInstaller
{
    public sealed class Installer
    {
        private Logger eventLogger;
        //private Dictionary<string, PackageManager> packManagers;

        //public DefaultPackagePathResolver LocalPathResolver { get; internal set; }

        public Installer(Logger eventLogger)
        {
            this.eventLogger = eventLogger;
            //packManagers = new Dictionary<string, PackageManager>();
        }

        public string Install(InstallationActionViewModel action)
        {
            var id = action.ExtensionId;
            var repositoryPath = action.ExtensionSource;

            action.SetProgress(InstallationProgress.Loading, "(ext-installer:progr-loading)");
            eventLogger.PushInfo($"Attempting to install extension '{id}' from '{repositoryPath}'");

            object packageInstaller = null;//GetInstallerManager(repositoryPath);
            if (packageInstaller is null)
            {
                action.SetProgress(InstallationProgress.Error, "(ext-installer:progr-error-repo-notfound)");

                return $"Can not install extension '{id}': repository '{repositoryPath}' not found";
            }

            object package = null;//packageInstaller.SourceRepository.FindPackage(id);
            if (package is null)
            {
                action.SetProgress(InstallationProgress.Error, "(ext-installer:progr-error-ext-notfound)");

                return $"Can not find extension with ID '{id}' on repository '{repositoryPath}'";
            }

            string error = null;//ExtensionsManager.VerifyPackageFrameworkDependencies(package);
            if (error.HasValue())
            {
                action.SetProgress(InstallationProgress.Error, "(ext-installer:progr-error-ext-cntinstall)");

                return $"Can not install extension with ID '{id}': {error}";
            }
            
            action.SetProgress(InstallationProgress.Installing, "(ext-installer:progr-installing)");
            try
            {
                //packageInstaller.InstallPackage(id);
                //string platform = ConfigurationManager.Instance["platform"];
                //string extensionPath = LocalPathResolver.GetPackageDirectory(package.Id, package.Version);
                //string root = ApplicationPaths.Instance[ApplicationPaths.EXTENSIONS];
                //string uiExtensionPath = FileManager.Combine(root, extensionPath, "lib", platform, $"{package.Id}.UI.dll");
            }
            catch (Exception ex)
            {
                action.SetProgress(InstallationProgress.Error, "(ext-installer:progr-error-ext-failinstall)");
                eventLogger.PushError(ex, null);

                return $"Extension '{id}': installation failed";
            }

            action.SetProgress(InstallationProgress.Success, "(ext-installer:progr-success)");
            eventLogger.PushInfo($"Extension '{id}' installed");
            
            return null;
        }

        //private PackageManager GetInstallerManager(string path)
        //{
        //    if (packManagers.TryGetValue(path, out PackageManager manager))
        //        return manager;
        //    var repository = PackageRepositoryFactory.Default.CreateRepository(path);
        //    if (repository is null)
        //        return null;
        //    manager = new PackageManager(repository, ApplicationPaths.Instance[ApplicationPaths.EXTENSIONS]);
        //    packManagers.Add(path, manager);
        //    return manager;
        //}
    }
}