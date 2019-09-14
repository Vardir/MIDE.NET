using System;

namespace XApp.Logging
{
    /// <summary>
    /// Base class for all application events, provides basic information -- logging level, register time and message
    /// </summary>
    public abstract class LoggedEvent
    {
        /// <summary>
        /// A level the event stands for
        /// </summary>
        public LoggingLevel Level { get; }
        /// <summary>
        /// A time when the event was registered
        /// </summary>
        public DateTime Time { get; }
        /// <summary>
        /// A brief description of event
        /// </summary>
        public string Message { get; }

        public LoggedEvent(LoggingLevel level, string message)
        {
            Time = DateTime.UtcNow;
            Level = level;
            Message = message;
        }
        public LoggedEvent(LoggingLevel level, string message, DateTime time)
        {
            Time = time;
            Level = level;
            Message = message;
        }

        public override string ToString() => $"{Time} [{Level}]:> {Message}"; //17:43:20 [DEBUG]:> My message 

        /// <summary>
        /// Extracts serialization data of the event
        /// </summary>
        /// <returns></returns>
        public abstract object[] GetSerializationData();
    }

    /// <summary>
    /// Represents application events for general purposes
    /// </summary>
    public class InfoEvent : LoggedEvent
    {
        public InfoEvent(string message) : base(LoggingLevel.INFO, message)
        {
             
        }
        public InfoEvent(string message, DateTime registerTime) : base(LoggingLevel.INFO, message, registerTime)
        {

        }

        public override object[] GetSerializationData() => null;
    }

    /// <summary>
    /// Represents additional information about application behavior for cases when that information is necessary to diagnose problems
    /// </summary>
    public class DebugEvent : LoggedEvent
    {
        /// <summary>
        /// Context the event assigned to
        /// </summary>
        public object Context { get; }

        public DebugEvent(object context, string message) : base(LoggingLevel.DEBUG, message)
        {
            Context = context;
        }
        public DebugEvent(object context, string message, DateTime registerTime)
            : base(LoggingLevel.DEBUG, message, registerTime)
        {
            Context = context;
        }

        /// <summary>
        /// Extracts serialization data of the event
        /// </summary>
        /// <returns></returns>
        public override object[] GetSerializationData() => new[] { Context };
    }

    /// <summary>
    /// Represents application events that may be an indication of a problem
    /// </summary>
    public class WarnEvent : LoggedEvent
    {
        public WarnEvent(string message) : base(LoggingLevel.WARN, message)
        {

        }
        public WarnEvent(string message, DateTime registerTime) : base(LoggingLevel.WARN, message, registerTime)
        {

        }

        /// <summary>
        /// Extracts serialization data of the event
        /// </summary>
        /// <returns></returns>
        public override object[] GetSerializationData() => null;
    }

    /// <summary>
    /// Typically includes the exception and contextual data when application exception occur
    /// </summary>
    public class ErrorEvent : LoggedEvent
    {
        /// <summary>
        /// Context the event assigned to
        /// </summary>
        public object Context { get; }
        /// <summary>
        /// An exception followed by the event
        /// </summary>
        public Exception Exception { get; }

        public ErrorEvent(Exception exception, object context) : base(LoggingLevel.ERROR, exception.Message)
        {
            Context = context;
            Exception = exception;
        }
        public ErrorEvent(Exception exception, object context, DateTime registerTime) 
            : base(LoggingLevel.ERROR, exception.Message, registerTime)
        {
            Context = context;
            Exception = exception;
        }
        public ErrorEvent(Exception exception, object context, string message) : base(LoggingLevel.ERROR, message)
        {
            Context = context;

            Exception = exception;
        }
        public ErrorEvent(Exception exception, object context, string message, DateTime registerTime) 
            : base(LoggingLevel.ERROR, message, registerTime)
        {
            Context = context;
            Exception = exception;
        }

        /// <summary>
        /// Extracts serialization data of the event
        /// </summary>
        /// <returns></returns>
        public override object[] GetSerializationData() => new[] { Exception, Context };
    }

    /// <summary>
    /// Represents a critical error that results in the termination of an application
    /// </summary>
    public class FatalEvent : LoggedEvent
    {
        public FatalEvent(string message) : base(LoggingLevel.FATAL, message)
        {

        }
        public FatalEvent(string message, DateTime registerTime) : base(LoggingLevel.WARN, message, registerTime)
        {

        }

        /// <summary>
        /// Extracts serialization data of the event
        /// </summary>
        /// <returns></returns>
        public override object[] GetSerializationData() => null;
    }
}