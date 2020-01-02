using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Vardirsoft.Shared.Helpers;

using Vardirsoft.XApp.IoC;
using Vardirsoft.XApp.API;

namespace Vardirsoft.XApp.Logging
{
    /// <summary>
    /// A logging service to register application events based on their logging levels
    /// </summary>
    public class Logger : ILogger
    {
        private readonly IFileManager _fileManager;
        private DateTime TimeNow { [DebuggerStepThrough] get => UseUtcTime ? DateTime.UtcNow : DateTime.Now; }
        private readonly LinkedList<LoggedEvent> _events;
        
        public bool UseUtcTime { get; }
        
        public LoggingLevel Levels { get; internal set; }
        
        public int EventsCount { [DebuggerStepThrough] get => _events.Count; }

        public event EventHandler<FatalEvent> FatalEventRegistered;

        public Logger(LoggingLevel levels, bool useUtcTime)
        {
            Levels = levels;
            UseUtcTime = useUtcTime;
            _events = new LinkedList<LoggedEvent>();
            _fileManager = IoCContainer.Resolve<IFileManager>();
        }
        
        public void SaveToFile(string folder, string fileName, string[] info)
        {
            var builder = new StringBuilder();
            if (info.HasValue())
            {
                foreach (var x in info)
                {
                    builder.Append(x);
                    builder.AppendLine();
                }
            }

            builder.Append("Is UTC time = " + UseUtcTime);
            builder.AppendLine();
            builder.Append("-------------------------");
            builder.AppendLine();

            var binIndex = 0;
            foreach (var item in Pull())
            {
                builder.Append(item);

                var serializationData = item.GetSerializationData();
                if (serializationData.HasValue())
                {
                    foreach (var x in serializationData)
                    {
                        if (x.HasValue())
                        {
                            builder.AppendLine();
                            builder.Append('\t');
                            builder.Append(x.GetType());
                            builder.Append("  - [");
                            builder.Append(binIndex + 1);
                            builder.Append(".bin]");

                            var binFile = _fileManager.Combine(folder, (binIndex + 1).ToString());
                            _fileManager.Serialize(x, $"{binFile}.bin");
                            binIndex++;
                        }
                    }
                }

                builder.AppendLine();
            }

            _fileManager.Write(builder.ToString(), _fileManager.Combine(folder, fileName));
        }
        
        public void PushEvent(LoggedEvent applicationEvent)
        {
            if (applicationEvent is null)
            {
                PushArgumentError(nameof(applicationEvent), "Can't register empty event");

                return;
            }

            if (Levels.Has(applicationEvent.Level))
            {   
                _events.AddLast(applicationEvent);
            }
        }
        
        public void PushInfo(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                PushArgumentError(nameof(message), "Can't add info event with empty message");

                return;
            }

            if (Levels.Has(LoggingLevel.INFO))
            {    
                _events.AddLast(new InfoEvent(message, TimeNow));
            }
        }
        
       public void PushDebug(object context, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                PushArgumentError(nameof(message), "Can't add debug event with empty message");

                return;
            }

            if (Levels.Has(LoggingLevel.DEBUG))
            {    
                _events.AddLast(new DebugEvent(context, message, TimeNow));
            }
        }
        
        public void PushWarning(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                PushArgumentError(nameof(message), "Can't add warning event with empty message");

                return;
            }

            if (Levels.Has(LoggingLevel.WARN))
            {    
                _events.AddLast(new WarnEvent(message));
            }
        }
        
        public void PushError(Exception exception, object context, string message = "")
        {
            if (exception is null)
            {
                PushArgumentError(nameof(exception), "Can't add error event without exception instance");

                return;
            }

            if (Levels.Has(LoggingLevel.ERROR))
            {    
                _events.AddLast(new ErrorEvent(exception, context, message, TimeNow));
            }
        }
        
        public void PushFatal(string message)
        {
            if (string.IsNullOrEmpty(message))
            {    
                PushArgumentError(nameof(message), "Adding fatal error event with empty message!");
            }

            var fatalEvent = new FatalEvent(message, TimeNow);
            _events.AddLast(fatalEvent);
            FatalEventRegistered?.Invoke(this, fatalEvent);
        }
        
        /// <summary>
        /// Filters out all events that are not set to the given levels
        /// </summary>
        /// <param name="levels"></param>
        public void FilterEvents(LoggingLevel levels)
        {
            var node = _events.First;
            while (node.HasValue())
            {
                var next = node.Next;
                if (!levels.HasFlag(node.Value.Level))
                {    
                    _events.Remove(node);
                }
                
                node = next;
            }
        }

        /// <summary>
        /// Returns all events based on the given levels
        /// </summary>
        /// <param name="levels"></param>
        /// <returns></returns>
        public IEnumerable<LoggedEvent> Pull(LoggingLevel levels = LoggingLevel.ALL) => _events.Where(appEvent => levels.Has(appEvent.Level));

        [DebuggerStepThrough]
        public void SetLoggingLevel(object sender, LoggingLevel level) => Levels = level;

        private void PushArgumentError(string argumentName, string message)
        {
            Exception exception = new ArgumentNullException(argumentName);
            PushError(exception, this, message);
        }
    }
}