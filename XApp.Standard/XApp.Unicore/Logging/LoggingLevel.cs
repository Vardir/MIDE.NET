using System;

namespace Vardirsoft.XApp.Logging
{
    [Flags]
    public enum LoggingLevel
    {
        NONE  = 0,
        /// <summary>
        /// Additional information about application behavior for cases when that information is necessary to diagnose problems
        /// </summary>
        DEBUG = 1,
        /// <summary>
        /// Application events for general purposes
        /// </summary>
        INFO = 2,
        /// <summary>
        /// Application events that may be an indication of a problem
        /// </summary>
        WARN = 4,
        /// <summary>
        /// Exception and contextual data when application exception occur
        /// </summary>
        ERROR = 8,
        /// <summary>
        /// Critical error that results in the termination of an application
        /// </summary>
        FATAL = 16,
        ALL   = DEBUG | INFO | WARN | ERROR | FATAL 
    }
}