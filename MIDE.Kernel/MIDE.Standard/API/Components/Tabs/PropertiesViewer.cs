using System;

namespace MIDE.API.Components
{
    public class PropertiesViewer : Tab
    {
        public override LayoutContainer ContentContainer
        {
            get => contentContainer;
            set
            {
                if (value == null)
                    return;
                if (!(value is PropertiesView))
                    throw new ArgumentException($"[PropertiesView] excepted but got [{value.GetType()}]");
                contentContainer = value;
                contentContainer.Parent = this;
                OnPropertyChanged(nameof(ContentContainer));
            }
        }

        public PropertiesViewer() : base("properties", false)
        {
            IsSealed = true;
        }

        public void SetView(PropertiesView view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));
            ContentContainer = view;
        }
    }
}