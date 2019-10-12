using System;
using System.Collections.Generic;

namespace Vardirsoft.XApp.Logging
{
    public interface ILogger
    {
        bool UseUtcTime { get; }
        LoggingLevel Levels { get; }
        int EventsCount { get; }

        event EventHandler<FatalEvent> FatalEventRegistered;

        void FilterEvents(LoggingLevel levels);
        IEnumerable<LoggedEvent> Pull(LoggingLevel levels = LoggingLevel.ALL);
        void PushDebug(object context, string message);
        void PushError(Exception exception, object context, string message = "");
        void PushEvent(LoggedEvent applicationEvent);
        void PushFatal(string message);
        void PushInfo(string message);
        void PushWarning(string message);
        void SaveToFile(string folder, string fileName, string[] info);
    }
}