namespace MIDE.Standard.API.Services
{
    /// <summary>
    /// Provides the bridge between application and buffer service (such as Windows system buffer) to store and retrieve data.
    /// </summary>
    public interface IBufferProvider
    {
        /// <summary>
        /// Pushes the value into the buffer's storage
        /// </summary>
        /// <param name="value"></param>
        void Push(object value);

        /// <summary>
        /// Loads the value stored in the buffer
        /// </summary>
        /// <returns></returns>
        object Pop();
    }
}