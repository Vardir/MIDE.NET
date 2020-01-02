using System.Diagnostics;

namespace Vardirsoft.XApp.Logging
{
    /// <summary>
    /// Local logging helpers class
    /// </summary>
    public static class LoggingHelpers
    {
        /// <summary>
        /// A type specific implementation of <seealso cref="System.Enum.HasFlag"/> method for <seealso cref="LoggingLevel"/>
        /// </summary>
        /// <param name="levels"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static bool Has(this LoggingLevel levels, LoggingLevel level) => (levels & level) == level;
    }
}