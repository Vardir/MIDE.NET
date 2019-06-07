using System;
using MIDE.FileSystem;

namespace MIDE.Application.Tasks
{
    [Serializable]
    public class ExecuteExtensionsInstallerTask : AppTask
    {
        public string InstallationConfigPath { get; set; }
        public string InstallerApplicationPath { get; set; }

        public ExecuteExtensionsInstallerTask(TaskRepetitionMode repetitionMode, string eventName) : base(repetitionMode, eventName)
        {
        }
        public ExecuteExtensionsInstallerTask(TaskActivationEvent activationEvent, 
                                              TaskRepetitionMode repetitionMode, string eventName = null) : base(activationEvent, repetitionMode, eventName)
        {
        }

        public override void Run(params object[] args)
        {
            AppKernel.Instance.AppLogger.PushInfo("Launching extensions installer");
            try
            {
                FileManager.ExecutionProvider.ExecuteExternalApplication(InstallerApplicationPath, InstallationConfigPath);
            }
            catch(Exception ex)
            {
                AppKernel.Instance.AppLogger.PushError(ex, this, "Unable to execute task");
            }
            AppKernel.Instance.AppLogger.PushInfo("Extensions installer stopped");
        }
    }
}