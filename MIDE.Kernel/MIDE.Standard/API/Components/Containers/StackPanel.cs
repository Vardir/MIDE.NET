using MIDE.API.Measurements;

namespace MIDE.API.Components
{
    public class StackPanel : Panel
    {
        private StackOrientation orientation;
        private BoundingBox childMargin;

        public BoundingBox ChildMargin
        {
            get => childMargin;
            set
            {
                if (childMargin == value)
                    return;
                childMargin = value;
                OnPropertyChanged(nameof(ChildMargin));
            }
        }
        public StackOrientation Orientation
        {
            get => orientation;
            set
            {
                if (orientation == value)
                    return;
                orientation = value;
                OnPropertyChanged(nameof(Orientation));
            }
        }

        public StackPanel(string id) : base(id) {}

        protected override Panel Create(string id)
        {
            StackPanel clone = new StackPanel(id);
            clone.orientation = orientation;
            clone.childMargin = childMargin;
            return clone;
        }
    }

    public enum StackOrientation
    {
        Horizontal, Vertical
    }
}