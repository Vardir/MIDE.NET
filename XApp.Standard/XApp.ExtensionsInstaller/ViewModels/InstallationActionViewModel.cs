using Vardirsoft.Shared.MVVM;
using Vardirsoft.XApp.API;
using Vardirsoft.XApp.Schemes.JSON;
using Vardirsoft.XApp.IoC;

namespace Vardirsoft.XApp.ExtensionsInstaller.ViewModels
{
    public class InstallationActionViewModel : BaseViewModel
    {
        private InstallationProgress _progress;
        private string _progressMessage;

        public bool AutoEnable { get; set; }
        public InstallationProgress Progress
        {
            get => _progress;
            set => SetWithNotify(ref _progress, value);
        }
        public InstallationMode InstallationMode { get; }
        public string ProgressMessage
        {
            get => _progressMessage;
            set => SetWithNotify(ref _progressMessage, IoCContainer.Resolve<ILocalizationProvider>()[value], false);
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