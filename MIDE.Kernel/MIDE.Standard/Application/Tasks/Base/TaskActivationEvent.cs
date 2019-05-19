namespace MIDE.Application.Tasks
{
    /// <summary>
    /// An enumeration that indicates when a task will be activated
    /// </summary>
    public enum TaskActivationEvent
    {
        /// <summary>
        /// Task is activated before application is loaded
        /// </summary>
        BeforeLoad,
        /// <summary>
        /// Task is activated on application load
        /// </summary>
        OnLoad,
        /// <summary>
        /// Task is activated on certain application event
        /// </summary>
        OnAppEvent,
        /// <summary>
        /// Task is activated on application exit
        /// </summary>
        OnExit
    }
}