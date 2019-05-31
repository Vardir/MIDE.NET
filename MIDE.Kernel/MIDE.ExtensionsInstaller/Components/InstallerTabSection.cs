using MIDE.Schemes.JSON;
using MIDE.API.Components;
using System.Collections.Generic;
using MIDE.ExtensionsInstaller.ViewModels;

namespace MIDE.ExtensionsInstaller.Components
{
    public class InstallerTabSection : TabSection
    {
        private static InstallerTabSection instance;
        public static InstallerTabSection Instance => instance ?? (instance = new InstallerTabSection());

        private InstallationPage installationPage;

        private InstallerTabSection() : base("installer")
        {
            installationPage = new InstallationPage();
            AddChild(installationPage);
            SelectedIndex = 0;
        }

        public void AddInstallationAction(string extensionId, string extensionSource, InstallationMode mode, bool autoEnable = true)
        {
            var actionViewModel = new InstallationActionViewModel(extensionId, extensionSource, mode);
            actionViewModel.AutoEnable = autoEnable;
            actionViewModel.SetProgress(InstallationProgress.Fetching, "(ext-installer:progr-fetching)");
            installationPage.Actions.Add(actionViewModel);
        }

        public IEnumerable<InstallationActionViewModel> GetInstallationActions() => installationPage.Actions;
    }
}