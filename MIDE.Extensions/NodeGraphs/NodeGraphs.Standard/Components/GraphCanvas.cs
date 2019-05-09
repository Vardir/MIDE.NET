using System;
using MIDE.API.Components;
using System.Collections.ObjectModel;

namespace NodeGraphs.Components
{
    public class GraphCanvas : LayoutContainer
    {
        public ObservableCollection<GraphComponent> Components { get; }

        public GraphCanvas(string id) : base(id)
        {
            Components = new ObservableCollection<GraphComponent>();
        }

        public override bool Contains(string id)
        {
            throw new NotImplementedException();
        }
        public override LayoutComponent Find(string id)
        {
            throw new NotImplementedException();
        }

        protected override LayoutComponent CloneInternal(string id)
        {
            throw new NotImplementedException();
        }

        protected override void AddChild_Impl(LayoutComponent component)
        {
            throw new NotImplementedException();
        }
        protected override void RemoveChild_Impl(string id)
        {
            throw new NotImplementedException();
        }
        protected override void RemoveChild_Impl(LayoutComponent component)
        {
            throw new NotImplementedException();
        }
    }
}