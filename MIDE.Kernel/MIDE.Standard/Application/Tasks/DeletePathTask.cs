using System;
using MIDE.FileSystem;

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
            var fileManager = FileManager.Instance;
            if (fileManager.FileExists(Path))
                fileManager.Delete(Path);
            else if (fileManager.DirectoryExists(Path))
                fileManager.DeleteDirectory(Path);
        }
    }
}