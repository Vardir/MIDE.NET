using MIDE.Schemes.JSON;
using MIDE.API.ViewModels;

namespace MIDE.ExtensionsInstaller.ViewModels
{
    public class InstallationActionViewModel : BaseViewModel
    {
        private InstallationProgress progress;
        private string progressMessage;

        public bool AutoEnable { get; set; }
        public InstallationProgress Progress
        {
            get => progress;
            set
            {
                if (value == progress)
                    return;
                progress = value;
                OnPropertyChanged(nameof(Progress));
            }
        }
        public InstallationMode InstallationMode { get; }
        public string ProgressMessage
        {
            get => progressMessage;
            set
            {
                if (value == progressMessage)
                    return;
                progressMessage = value;
                OnPropertyChanged(nameof(ProgressMessage));
            }
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