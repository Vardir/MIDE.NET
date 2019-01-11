﻿namespace MIDE.API.Components
{
    /// <summary>
    /// A tab item used to contain a group of components
    /// </summary>
    public class Tab : LayoutContainer
    {
        private string header;
        private LayoutContainer contentContainer;

        public string Header
        {
            get => header;
            set
            {
                if (value == header)
                    return;
                header = value;
                OnPropertyChanged(nameof(Header));
            }
        }
        public LayoutContainer ContentContainer
        {
            get => contentContainer;
            set
            {
                if (value == null)
                    return;
                contentContainer = value;
                contentContainer.Parent = this;
                OnPropertyChanged(nameof(ContentContainer));
            }
        }

        public Tab(string id) : base(id)
        {
            Header = FormatId();
        }

        public override bool Contains(string id) => contentContainer?.Contains(id) ?? false;
        public override LayoutComponent Find(string id) => contentContainer?.Find(id);

        protected override void AddChild_Impl(LayoutComponent component) => contentContainer?.AddChild(component);
        protected override void RemoveChild_Impl(string id) => contentContainer?.RemoveChild(id);
        protected override void RemoveChild_Impl(LayoutComponent component) => contentContainer?.RemoveChild(component);
    }
}