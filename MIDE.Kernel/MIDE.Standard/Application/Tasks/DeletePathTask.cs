using System;

namespace MIDE.Application.Tasks
{
    [Serializable]
    public class DeletePathTask : AppTask
    {
        public string Path { get; set; }

        public DeletePathTask(TaskRepetitionMode repetitionMode, string eventName) : base(repetitionMode, eventName)
        {
        }
        public DeletePathTask(TaskActivationEvent activationEvent, TaskRepetitionMode repetitionMode, string eventName = null) : base(activationEvent, repetitionMode, eventName)
        {
        }

        public override void Run(params object[] args)
        {
            if (AppKernel.Instance.FileManager.IsFile(Path))
                AppKernel.Instance.FileManager.Delete(Path);
            else if (AppKernel.Instance.FileManager.IsDirectory(Path))
                AppKernel.Instance.FileManager.DeleteDirectory(Path);
        }
    }
}