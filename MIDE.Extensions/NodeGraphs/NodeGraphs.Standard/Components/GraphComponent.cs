using System;
using MIDE.API.Components;

namespace NodeGraphs.Components
{
    public abstract class GraphComponent : LayoutComponent
    {
        public GraphCanvas ParentCanvas { get; internal set; }

        public GraphComponent(string id) : base(id)
        {

        }
        public GraphComponent(string id, GraphCanvas canvas) : this(id)
        {
            ParentCanvas = canvas;
        }

        protected override LayoutComponent CloneInternal(string id)
        {
            throw new NotImplementedException();
        }
    }
}