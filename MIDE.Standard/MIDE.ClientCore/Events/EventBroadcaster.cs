using MIDE.Helpers;
using MIDE.Logging;
using System.Collections.Generic;

namespace MIDE.Application.Events
{
    public class EventBroadcaster
    {
        private readonly Dictionary<string, LinkedList<IEventListener>> groups;

        public bool WriteLog { get; set; }
        public Logger Logger { get; }

        public EventBroadcaster()
        {
            groups = new Dictionary<string, LinkedList<IEventListener>>();
            Logger = new Logger(LoggingLevel.INFO | LoggingLevel.WARN, useUtcTime: true);
        }

        public void Broadcast(string group, object sender, object message)
        {
            if (WriteLog)
            {
                Logger.PushInfo($"{sender} sent message to <{group}>");
            }
            if (groups.TryGetValue(group, out var listeners))
            {
                foreach (var listener in listeners)
                {
                    listener.Receive(sender, message);
                }
                if (WriteLog)
                    Logger.PushInfo($"{listeners.Count} listeners received last message");
            }
            else
            {
                if (WriteLog)
                    Logger.PushInfo("No listeners received last message");
            }
        }
        public void AddListener(IEventListener listener, string group)
        {
            if (listener == null)
            {
                Logger.PushWarning("Attempt to add empty listener");
                return;
            }
            if (group == null)
            {
                Logger.PushWarning("Attempt to add listener to group");
                return;
            }

            if (!groups.ContainsKey(group))
                groups.Add(group, new LinkedList<IEventListener>());

            groups[group].AddLast(listener);
        }
        public void RemoveListener(IEventListener listener)
        {
            if (listener.HasValue())
            {
                foreach (var kvp in groups)
                {
                    if (kvp.Value.Remove(listener))
                    {
                        if (WriteLog)
                            Logger.PushInfo($"Listener {listener.Id} removed from <{kvp.Key}>");
                    }
                }
            }
            else
            {
                Logger.PushWarning("Attempt to remove empty listener");
            }
        }
        public void RemoveListener(IEventListener listener, string group)
        {
            if (listener.HasValue())
            {
                if (group.HasValue() && groups.ContainsKey(group))
                {
                    if (groups[group].Remove(listener) && WriteLog)
                        Logger.PushInfo($"Listener {listener.Id} removed from <{group}>");
                }
                else
                {
                    Logger.PushWarning("Attempt to remove listener from non existing group");
                }
            }
            else
            {
                Logger.PushWarning("Attempt to remove empty listener");
                return;
            }
        }
    }
}