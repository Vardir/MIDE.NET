using MIDE.API.Validations;
using System.Collections.ObjectModel;

namespace NodeGraphs.Components
{
    public abstract class GraphNode : GraphComponent
    {
        private Point location;

        public Point Location
        {
            get => location;
            set
            {
                if (value == location)
                    return;
                location = value;
                OnPropertyChanged(nameof(Location));
            }
        }
        public IGraphNodeContentProvider ContentProvider { get; }
        public ObservableCollection<ValidationError> ValidationErrors { get; }

        public GraphNode(string id, IGraphNodeContentProvider contentProvider) : base(id)
        {
            ContentProvider = contentProvider;
            ValidationErrors = new ObservableCollection<ValidationError>();
        }

        protected void AddError(string message, object invalidValue = null, string property = null)
        {
            ValidationErrors.Add(new ValidationError(property, message, invalidValue));
        }
    }
}