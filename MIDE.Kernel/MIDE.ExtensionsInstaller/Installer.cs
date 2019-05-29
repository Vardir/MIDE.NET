using NuGet;
using System;
using MIDE.FileSystem;
using MIDE.Application.Logging;
using System.Collections.Generic;
using MIDE.ExtensionsInstaller.ViewModels;
using System.Linq;
using Newtonsoft.Json;
using MIDE.Schemes.JSON;
using MIDE.Helpers;
using MIDE.Application.Configuration;

namespace MIDE.ExtensionsInstaller
{
    public sealed class Installer
    {
        private Logger eventLogger;
        private FileManager fileManager;
        private Dictionary<string, PackageManager> packManagers;

        public Installer(Logger eventLogger)
        {
            this.eventLogger = eventLogger;
            fileManager = FileManager.Instance;
            packManagers = new Dictionary<string, PackageManager>();
        }

        public string Install(InstallationActionViewModel action)
        {
            string id = action.ExtensionId;
            string repositoryPath = action.ExtensionSource;
            action.SetProgress(InstallationProgress.Loading, "Loading extension from repository");
            eventLogger.PushInfo($"Attempting to install extension '{id}' from '{repositoryPath}'");
            var packageInstaller = GetInstallerManager(repositoryPath);
            if (packageInstaller == null)
            {
                action.SetProgress(InstallationProgress.Error, "Repository not found or it is corrupted");
                return $"Can not install extension '{id}': repository '{repositoryPath}' not found";
            }
            var package = packageInstaller.SourceRepository.FindPackage(id);
            if (package == null)
            {
                action.SetProgress(InstallationProgress.Error, "Extension not found");
                return $"Can not find extension with ID '{id}' on repository '{repositoryPath}'";
            }
            string error = VerifyPackageDependencies(package);
            if (error != null)
            {
                action.SetProgress(InstallationProgress.Error, "Extension can not be installed");
                return $"Can not install extension with ID '{id}': {error}";
            }
            action.SetProgress(InstallationProgress.Installing, "Installing extension");
            try
            {
                packageInstaller.InstallPackage(id);
            }
            catch (Exception ex)
            {
                action.SetProgress(InstallationProgress.Error, "Extension installation failed, see log");
                eventLogger.PushError(ex, null);
                return $"Extension '{id}': installation failed";
            }
            action.SetProgress(InstallationProgress.Success, "Extension installed");
            eventLogger.PushInfo($"Extension '{id}' installed");
            return null;
        }

        private string VerifyPackageDependencies(IPackage package)
        {
            var frameworkAssemblies = package.FrameworkAssemblies.Where(assembly => assembly.AssemblyName[0] == '!').ToList();
            foreach (var assembly in frameworkAssemblies)
            {
                var (name, part) = assembly.AssemblyName.ExtractUntil(1, ':');
                Version version = Version.Parse(part);
                string entry = ConfigurationManager.Instance[name] as string;
                Version version2 = entry != null ? Version.Parse(entry) : null;
                if (version2 == null)
                    return $"Application does not meet extension's requirements on '{name}' of {version} version";
                if (version2 < version)
                    return $"Extension requires '{name}' of {version} version but application got {version2}";
            }
            return null;

        }
        private PackageManager GetInstallerManager(string path)
        {
            if (packManagers.TryGetValue(path, out PackageManager manager))
                return manager;
            var repository = PackageRepositoryFactory.Default.CreateRepository(path);
            if (repository == null)
                return null;
            manager = new PackageManager(repository, ApplicationPaths.Instance[ApplicationPaths.EXTENSIONS]);
            packManagers.Add(path, manager);
            return manager;
        }
    }
}