using System;

namespace MIDE.Application.Tasks
{
    /// <summary>
    /// A task that gives simple instruction to application to accomplish on certain event
    /// </summary>
    [Serializable]
    public abstract class AppTask
    {
        public TaskActivationEvent ActivationEvent { get; }
        public TaskRepetitionMode RepetitionMode { get; }
        public string EventName { get; }
        public string Origin { get; set; }

        public AppTask(TaskRepetitionMode repetitionMode, string eventName)
        {
            EventName = eventName;
            RepetitionMode = repetitionMode;
            ActivationEvent = TaskActivationEvent.OnAppEvent;
        }
        public AppTask(TaskActivationEvent activationEvent, TaskRepetitionMode repetitionMode, string eventName = null)
        {
            EventName = eventName;
            RepetitionMode = repetitionMode;
            ActivationEvent = activationEvent;
        }

        /// <summary>
        /// Runs the task
        /// </summary>
        /// <param name="args"></param>
        public abstract void Run(params object[] args);
    }
}