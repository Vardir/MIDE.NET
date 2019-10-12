using System;

using Vardirsoft.XApp.IoC;
using Vardirsoft.XApp.Logging;
using Vardirsoft.XApp.FileSystem;

namespace Vardirsoft.XApp.Application.Tasks
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
            var logger = IoCContainer.Resolve<ILogger>();
            logger.PushInfo("Launching extensions installer");

            try
            {
                IoCContainer.Resolve<IExecutionProvider>().ExecuteExternalApplication(InstallerApplicationPath, InstallationConfigPath);
            }
            catch(Exception ex)
            {
                logger.PushError(ex, this, "Unable to execute task");
            }

            logger.PushInfo("Extensions installer stopped");
        }
    }
}