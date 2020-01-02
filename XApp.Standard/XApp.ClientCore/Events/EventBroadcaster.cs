using System.Collections.Generic;
using System.Linq;

using Vardirsoft.Shared.Helpers;

using Vardirsoft.XApp.Logging;

namespace Vardirsoft.XApp.Application.Events
{
    public class EventBroadcaster
    {
        private readonly Dictionary<string, LinkedList<IEventListener>> _groups;

        public bool WriteLog { get; set; }
        
        public Logger Logger { get; }

        public EventBroadcaster()
        {
            _groups = new Dictionary<string, LinkedList<IEventListener>>();
            Logger = new Logger(LoggingLevel.INFO | LoggingLevel.WARN, useUtcTime: true);
        }

        public void Broadcast(string group, object sender, object message)
        {
            if (WriteLog)
            {
                Logger.PushInfo($"{sender} sent message to <{group}>");
            }
            
            if (_groups.TryGetValue(group, out var listeners))
            {
                listeners.ForEach(x => x.Receive(sender, message));

                if (WriteLog)
                {
                    Logger.PushInfo($"{listeners.Count} listeners received last message");
                }
            }
            else
            {
                if (WriteLog)
                {    
                    Logger.PushInfo("No listeners received last message");
                }
            }
        }
        
        public void AddListener(IEventListener listener, string group)
        {
            if (listener is null)
            {
                Logger.PushWarning("Attempt to add empty listener");

                return;
            }
            if (group is null)
            {
                Logger.PushWarning("Attempt to add listener to group");

                return;
            }

            if (!_groups.ContainsKey(group))
            {    
                _groups.Add(group, new LinkedList<IEventListener>());
            }

            _groups[group].AddLast(listener);
        }
        
        public void RemoveListener(IEventListener listener)
        {
            if (listener.HasValue())
            {
                foreach (var kvp in _groups.Where(kvp => kvp.Value.Remove(listener)).Where(kvp => WriteLog))
                {
                    Logger.PushInfo($"Listener {listener.Id} removed from <{kvp.Key}>");
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
                if (group.HasValue() && _groups.ContainsKey(group))
                {
                    if (_groups[group].Remove(listener) && WriteLog)
                    {   
                        Logger.PushInfo($"Listener {listener.Id} removed from <{group}>");
                    }
                }
                else
                {
                    Logger.PushWarning("Attempt to remove listener from non existing group");
                }
            }
            else
            {
                Logger.PushWarning("Attempt to remove empty listener");
            }
        }
    }
}