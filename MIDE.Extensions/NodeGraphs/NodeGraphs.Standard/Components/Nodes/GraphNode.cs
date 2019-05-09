using MIDE.API.Validations;
using System.Collections.ObjectModel;

namespace NodeGraphs.Components
{
    public abstract class GraphNode : GraphComponent
    {
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