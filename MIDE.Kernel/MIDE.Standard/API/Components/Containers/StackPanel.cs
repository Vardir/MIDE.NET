using MIDE.Standard.API.Measurements;

namespace MIDE.Standard.API.Components
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
    }

    public enum StackOrientation
    {
        Horizontal, Vertical
    }
}