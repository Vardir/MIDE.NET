namespace MIDE.API.Converters
{
    /// <summary>
    /// A two way converter interface
    /// </summary>
    public interface IValueConverter
    {
        /// <summary>
        /// Converts the value in direct conversion
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        object Convert(object value);
        /// <summary>
        /// Converts the value in backward directed conversion
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        object ConvertBack(object value);
    }
}