namespace MIDE.Components
{
    public class PropertiesViewer : Tab
    {
        protected PropertiesView view;

        public PropertiesView View
        {
            get => view;
            set
            {
                if (value == null)
                    return;
                if (view != null)
                    RemoveChild(view);
                view = value;
                view.Parent = this;
                AddChild(view);
                OnPropertyChanged(nameof(View));
            }
        }

        public PropertiesViewer(string id) : base(id, false)
        {
            
        }

        protected override Tab Create(string id, Toolbar toolbar, bool allowDuplicates)
        {
            PropertiesViewer clone = new PropertiesViewer(id);
            view = view.Clone() as PropertiesView;
            return clone;
        }
    }
}