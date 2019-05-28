using System;
using System.Text;
using MIDE.FileSystem;
using System.Collections.Generic;

namespace MIDE.Application.Logging
{
    /// <summary>
    /// A logging service to register application events based on their logging levels
    /// </summary>
    public class Logger
    {
        private DateTime TimeNow => UseUtcTime ? DateTime.UtcNow : DateTime.Now;
        private LinkedList<LoggedEvent> events;

        /// <summary>
        /// A flag to indicate whether to use UTC time for registering events
        /// </summary>
        public bool UseUtcTime { get; }
        /// <summary>
        /// A set of flags to filter out incoming events
        /// </summary>
        public LoggingLevel Levels { get; internal set; }
        /// <summary>
        /// Count of all events stored in collection
        /// </summary>
        public int EventsCount => events.Count;

        public event EventHandler<FatalEvent> FatalEventRegistered;

        public Logger (LoggingLevel levels, bool useUtcTime)
        {
            Levels = levels;
            UseUtcTime = useUtcTime;
            events = new LinkedList<LoggedEvent>();
        }

        /// <summary>
        /// Saves log entries into the given file
        /// </summary>
        /// <param name="path"></param>
        public void SaveToFile(string folder, string fileName, string[] info)
        {
            var fileManager = FileManager.Instance;
            StringBuilder builder = new StringBuilder();
            if (info != null)
            {
                for (int i = 0; i < info.Length; i++)
                {
                    builder.Append(info[i]);
                    builder.AppendLine();
                }
            }
            builder.Append("Is UTC time = " + UseUtcTime);
            builder.AppendLine();
            builder.Append("-------------------------");
            builder.AppendLine();
            int binIndex = 0;
            foreach (var item in Pull())
            {
                builder.Append(item.ToString());
                object[] serializationData = item.GetSerializationData();
                if (serializationData != null)
                {
                    for (int i = 0; i < serializationData.Length; i++)
                    {
                        if (serializationData[i] == null)
                            continue;
                        builder.AppendLine();
                        builder.Append('\t');
                        builder.Append(serializationData[i].GetType());
                        builder.Append("  - [");
                        builder.Append(binIndex + 1);
                        builder.Append(".bin]");
                        string binFile = fileManager.Combine(folder, (binIndex + 1).ToString());
                        fileManager.Serialize(serializationData[i], $"{binFile}.bin");
                        binIndex++;
                    }
                }
                builder.AppendLine();
            }
            fileManager.Write(builder.ToString(), fileManager.Combine(folder, fileName));
        }

        /// <summary>
        /// Adds the given event into collection
        /// </summary>
        /// <param name="applicationEvent"></param>
        public void PushEvent(LoggedEvent applicationEvent)
        {
            if (applicationEvent == null)
            {
                PushArgumentError(nameof(applicationEvent), "Can't register empty event");
                return;
            }
            if (!Levels.Has(applicationEvent.Level))
                return;
            events.AddLast(applicationEvent);
        }
        /// <summary>
        /// Adds an informational event into collection
        /// </summary>
        /// <param name="message"></param>
        public void PushInfo(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                PushArgumentError(nameof(message), "Can't add info event with empty message");
                return;
            }
            if (!Levels.Has(LoggingLevel.INFO))
                return;
            events.AddLast(new InfoEvent(message, TimeNow));
        }
        /// <summary>
        /// Adds a debug event into collection based on context
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        public void PushDebug(object context, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                PushArgumentError(nameof(message), "Can't add debug event with empty message");
                return;
            }
            if (!Levels.Has(LoggingLevel.DEBUG))
                return;
            events.AddLast(new DebugEvent(context, message, TimeNow));
        }
        /// <summary>
        /// Adds a warning event into collection
        /// </summary>
        /// <param name="message"></param>
        public void PushWarning(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                PushArgumentError(nameof(message), "Can't add warning event with empty message");
                return;
            }
            if (!Levels.Has(LoggingLevel.WARN))
                return;
            events.AddLast(new WarnEvent(message));
        }
        /// <summary>
        /// Adds an error event with exception instance
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="context"></param>
        /// <param name="message"></param>
        public void PushError(Exception exception, object context, string message = "")
        {
            if (exception == null)
            {
                PushArgumentError(nameof(exception), "Can't add error event without exception instance");
                return;
            }
            if (!Levels.Has(LoggingLevel.ERROR))
                return;
            events.AddLast(new ErrorEvent(exception, context, message, TimeNow));
        }
        /// <summary>
        /// Adds a fatal error event with description of problem occur
        /// </summary>
        /// <param name="message"></param>
        public void PushFatal(string message)
        {
            if (string.IsNullOrEmpty(message))
                PushArgumentError(nameof(message), "Adding fatal error event with empty message!");
            
            FatalEvent fatalEvent = new FatalEvent(message, TimeNow);
            events.AddLast(fatalEvent);
            FatalEventRegistered?.Invoke(this, fatalEvent);
        }
        /// <summary>
        /// Filters out all events that are not set to the given levels
        /// </summary>
        /// <param name="levels"></param>
        public void FilterEvents(LoggingLevel levels)
        {
            var node = events.First;
            while (node != null)
            {
                var next = node.Next;
                if (!levels.HasFlag(node.Value.Level))
                    events.Remove(node);
                node = next;
            }
        }

        /// <summary>
        /// Returns all events based on the given levels
        /// </summary>
        /// <param name="levels"></param>
        /// <returns></returns>
        public IEnumerable<LoggedEvent> Pull(LoggingLevel levels = LoggingLevel.ALL)
        {
            foreach (LoggedEvent appEvent in events)
            {
                if (levels.Has(appEvent.Level))
                    yield return appEvent;
            }
        }

        private void PushArgumentError(string argumentName, string message)
        {
            Exception exception = new ArgumentNullException(argumentName);
            PushError(exception, this, message);
        }
    }
}