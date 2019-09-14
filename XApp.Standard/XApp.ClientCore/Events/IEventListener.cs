namespace XApp.Application.Events
{
    /// <summary>
    /// Provides access to subscribe on events from broadcasters
    /// </summary>
    public interface IEventListener
    {
        /// <summary>
        /// Listener ID which is used in broadcaster's logs
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Receives and operates on events provided by broadcaster
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        void Receive(object sender, object message);
    }
}