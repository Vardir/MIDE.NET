namespace MIDE.API.Components
{
    public abstract class PropertiesView : GridLayout
    {
        public PropertiesView(string id) : base(id) { }

        public abstract void ShowFor(object context);
    }
}