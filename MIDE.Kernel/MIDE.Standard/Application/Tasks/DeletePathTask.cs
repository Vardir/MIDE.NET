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
            if (FileManager.FileExists(Path))
                FileManager.Delete(Path);
            else if (FileManager.DirectoryExists(Path))
                FileManager.DeleteDirectory(Path);
        }
    }
}