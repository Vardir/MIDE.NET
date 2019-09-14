using XApp.API;
using XApp.Schemes.JSON;
using XApp.Application.Localization;

namespace XApp.ExtensionsInstaller.ViewModels
{
    public class InstallationActionViewModel : BaseViewModel
    {
        private InstallationProgress progress;
        private string progressMessage;

        public bool AutoEnable { get; set; }
        public InstallationProgress Progress
        {
            get => progress;
            set => SetAndNotify(value, ref progress);
        }
        public InstallationMode InstallationMode { get; }
        public string ProgressMessage
        {
            get => progressMessage;
            set => SetAndNotify(LocalizationProvider.Instance[value], ref progressMessage);
        }
        public string ExtensionId { get; }
        public string ExtensionSource { get; }

        public InstallationActionViewModel(string extensionId, string extensionSource, InstallationMode mode)
        {
            InstallationMode = mode;
            ExtensionId = extensionId;
            ExtensionSource = extensionSource;
        }

        public void SetProgress(InstallationProgress progress, string message)
        {
            Progress = progress;
            ProgressMessage = message;
        }
    }
}