﻿namespace MIDE.Components
{
    public class ToolbarButton : Button, IToolBarItem
    {
        public int Order { get; set; }
        public string Group { get; set; }

        public ToolbarButton(string id) : base(id) {}

        protected override Button Create(string id)
        {
            var clone = new ToolbarButton(id);
            clone.Order = Order;
            clone.Group = Group;
            
            return clone;
        }
    }
}