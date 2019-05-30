﻿using NuGet;
using System;
using MIDE.FileSystem;
using MIDE.Application;
using MIDE.Application.Logging;
using System.Collections.Generic;
using MIDE.Application.Configuration;
using MIDE.ExtensionsInstaller.ViewModels;

namespace MIDE.ExtensionsInstaller
{
    public sealed class Installer
    {
        private Logger eventLogger;
        private FileManager fileManager;
        private Dictionary<string, PackageManager> packManagers;

        public DefaultPackagePathResolver LocalPathResolver { get; internal set; }

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
            string error = ExtensionsManager.VerifyPackageFrameworkDependencies(package);
            if (error != null)
            {
                action.SetProgress(InstallationProgress.Error, "Extension can not be installed");
                return $"Can not install extension with ID '{id}': {error}";
            }
            action.SetProgress(InstallationProgress.Installing, "Installing extension");
            try
            {
                packageInstaller.InstallPackage(id);
                string platform = ConfigurationManager.Instance["platform"];
                string extensionPath = LocalPathResolver.GetPackageDirectory(package.Id, package.Version);
                string root = ApplicationPaths.Instance[ApplicationPaths.EXTENSIONS];
                string uiExtensionPath = fileManager.Combine(root, extensionPath, "lib", platform, $"{package.Id}.UI.dll");
                if (fileManager.FileExists(uiExtensionPath))
                    fileManager.Copy(uiExtensionPath, fileManager.Combine(extensionPath, $"{package.Id}.UI.dll"));
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