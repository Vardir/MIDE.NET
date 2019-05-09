using MIDE.API.Validations;
using System.Collections.Generic;

namespace NodeGraphs.Components
{
    public abstract class ConstantNode<T> : GraphNode
    {
        private T value;

        public T Value
        {
            get => value;
            set
            {
                if (this.value != null && this.value.Equals(value))
                    return;
                ValidationErrors.Clear();
                Validations.ForEach(v => v.Validate(value, nameof(Value), ValidationErrors));
                if (ValidationErrors.Count != 0)
                    return;
                this.value = value;
                Output.Push(value);
                OnPropertyChanged(nameof(Value));
            }
        }
        public T DefaultValue { get; }
        public NodeJoint<T> Output { get; protected set; }
        public List<ValueValidation<T>> Validations { get; }

        public ConstantNode(string id, T defaultValue, IGraphNodeContentProvider contentProvider = null) : base(id, contentProvider)
        {
            value = defaultValue;
            DefaultValue = defaultValue;
            Validations = new List<ValueValidation<T>>();
        }

        public void Clear()
        {
            if (!IsEnabled)
                return;
            Value = DefaultValue;
        }
    }
}