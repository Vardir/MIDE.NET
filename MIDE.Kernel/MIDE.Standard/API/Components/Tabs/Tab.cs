using MIDE.API.Commands;

namespace MIDE.API.Components
{
    /// <summary>
    /// A tab item used to contain a group of components
    /// </summary>
    public class Tab : LayoutContainer
    {
        private string header;

        protected LayoutContainer contentContainer;

        public bool AllowDuplicates { get; }
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
        public TabSection ParentSection { get; internal set; }
        public Toolbar TabToolbar { get; }
        public virtual LayoutContainer ContentContainer
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
        public ICommand CloseCommand { get; set; }

        private Tab(string id, Toolbar toolbar, bool allowDuplicates) : base(id)
        {
            TabToolbar = toolbar ?? new Toolbar("toolbar");
            TabToolbar.Parent = this;
            AllowDuplicates = allowDuplicates;
            Header = FormatId();
        }

        public Tab(string id, bool allowDuplicates = false) : this(id, null, allowDuplicates) { }

        public override bool Contains(string id) => contentContainer?.Contains(id) ?? false;
        public override LayoutComponent Find(string id) => contentContainer?.Find(id);

        protected override LayoutComponent CloneInternal(string id)
        {
            Tab clone = new Tab(id, (Toolbar)TabToolbar.Clone(), AllowDuplicates);
            clone.header = header;
            clone.ParentSection = null;
            clone.CloseCommand = null;
            clone.contentContainer = (LayoutContainer)contentContainer.Clone();
            return clone;
        }

        protected override void AddChild_Impl(LayoutComponent component) => contentContainer?.AddChild(component);
        protected override void RemoveChild_Impl(string id) => contentContainer?.RemoveChild(id);
        protected override void RemoveChild_Impl(LayoutComponent component) => contentContainer?.RemoveChild(component);
    }
}