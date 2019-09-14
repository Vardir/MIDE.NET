namespace XApp.Application.Tasks
{
    /// <summary>
    /// An enumeration that describes how much times application will run a task
    /// </summary>
    public enum TaskRepetitionMode
    {
        /// <summary>
        /// A task will be used once and removed forever
        /// </summary>
        Once,
        /// <summary>
        /// A task will be used once and removed until the next application execution
        /// </summary>
        OncePerExecution,
        /// <summary>
        /// A task will be used more than once and saved for next application executions
        /// </summary>
        NotLimited,
        /// <summary>
        /// A task will be used more than once per application execution but will not be saved for next executions
        /// </summary>
        NotLimitedOnce
    }
}