using System;
using MIDE.Helpers;
using MIDE.API.Components;
using NodeGraphs.DataModels;
using System.Collections.ObjectModel;

namespace NodeGraphs.Components
{
    public class GraphCanvas : LayoutContainer
    {
        public Axis XAxis { get; }
        public Axis YAxis { get; }

        /// <summary>
        /// WARNING: do not modify this collection directly as it is opened for UI binding only
        /// </summary>
        public ObservableCollection<GraphComponent> Components { get; }

        public GraphCanvas(string id) : base(id)
        {
            XAxis = new Axis();
            YAxis = new Axis();
            Components = new ObservableCollection<GraphComponent>();
        }

        public override bool Contains(string id) => Components.Find(gc => gc.Id == id) != null;
        public override LayoutComponent Find(string id) => Components.Find(gc => gc.Id == id);

        protected override LayoutComponent CloneInternal(string id)
        {
            GraphCanvas clone = new GraphCanvas(id);
            foreach (var component in Components)
            {
                clone.AddChild(component.Clone());
            }
            return clone;
        }

        protected override void AddChild_Impl(LayoutComponent component)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));
            if (!(component is GraphComponent gc))
                throw new ArgumentException($"Expected [{typeof(GraphComponent)}] but got [{typeof(LayoutComponent)}]");
            gc.Parent = this;
            gc.ParentCanvas = this;
            Components.Add(gc);
        }
        protected override void RemoveChild_Impl(string id)
        {
            int index = Components.FirstIndexWith(gc => gc.Id == id);
            if (index == -1)
                return;
            GraphComponent component = Components[index];
            component.Parent = null;
            component.ParentCanvas = null;
            Components.RemoveAt(index);
        }
        protected override void RemoveChild_Impl(LayoutComponent component)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));
            if (!(component is GraphComponent gc))
                throw new ArgumentException($"Canvas does not contain items of type [{typeof(LayoutComponent)}]");
            if (!Components.Remove(gc))
                return;
            gc.Parent = null;
            gc.ParentCanvas = null;
        }
    }
}