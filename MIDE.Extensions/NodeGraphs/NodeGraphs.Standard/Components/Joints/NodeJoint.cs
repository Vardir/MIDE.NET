using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NodeGraphs.Components
{
    public abstract class NodeJoint : INotifyPropertyChanged
    {
        protected List<GraphConnection> connections;

        public bool CanConnect => MaxConnections == -1 || connections.Count < MaxConnections;
        public int MaxConnections { get; }
        public int ConnectionsCount => connections.Count;
        public JointRole Role { get; }
        public GraphNode Parent { get; }
        public IEnumerable<GraphConnection> Connections => connections;

        public event Action Pulsed;
        public event PropertyChangedEventHandler PropertyChanged;

        public NodeJoint(GraphNode parent, JointRole role, int maxConnections)
        {
            Role = role;
            Parent = parent;
            MaxConnections = maxConnections;
            connections = new List<GraphConnection>();
        }

        public void Pulse() => Pulsed();
        public void AddConnection(GraphConnection connection)
        {
            connections.Add(connection);
            if (Role == JointRole.Input)
                Pulse();
            OnPropertyChanged(nameof(ConnectionsCount));
        }
        public void RemoveConnection(GraphConnection connection)
        {
            connections.Remove(connection);
            if (Role == JointRole.Input)
                Pulse();
            OnPropertyChanged(nameof(ConnectionsCount));
        }

        public abstract object GetValue();

        protected void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }

    public abstract class NodeJoint<T> : NodeJoint
    {
        public T Value { get; private set; }

        public NodeJoint(GraphNode parent, JointRole role, int maxConnections) : base(parent, role, maxConnections)
        {
            
        }

        public void Push(T value)
        {
            if (Role != JointRole.Output)
                return;
            Value = value;
            for (int i = 0; i < connections.Count; i++)
            {
                connections[i].InputJoint.Pulse();
            }
        }
    }
}