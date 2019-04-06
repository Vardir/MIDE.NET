namespace MIDE.API.Components
{
    public abstract class PropertiesView : LayoutContainer
    {
        public PropertiesView(string id) : base(id) { }

        public abstract void ShowFor(object context);
    }
}