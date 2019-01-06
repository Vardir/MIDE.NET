namespace MIDE.API.Components
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
                OnPropertyChanged(nameof(ContentContainer));
            }
        }

        public Tab(string id) : base(id)
        {

        }

        public override void AddChild(LayoutComponent component) => contentContainer?.AddChild(component);
        public override void RemoveChild(string id) => contentContainer?.RemoveChild(id);
        public override void RemoveChild(LayoutComponent component) => contentContainer?.RemoveChild(component);

        public override bool Contains(string id) => contentContainer?.Contains(id) ?? false;
        public override LayoutComponent Find(string id) => contentContainer?.Find(id);
    }
}