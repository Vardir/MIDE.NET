using System.ComponentModel;

namespace MIDE.API.Visuals
{
    /// <summary>
    /// Implement this interface to provide possibility to add graphical information your application components 
    /// </summary>
    public class Glyph : INotifyPropertyChanged
    {
        private object value;
        public object Value
        {
            get => value;
            set
            {
                if (this.value == value)
                    return;
                if (!Validate(value))
                    return;
                this.value = value;
                OnPropertyChanged(nameof(Value));
            }
        }
        public GlyphKind Kind { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public Glyph(GlyphKind kind)
        {
            Kind = kind;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool Validate(object value)
        {
            switch (Kind)
            {
                case GlyphKind.Char:
                    return value is char;
                case GlyphKind.String:
                    if (value is string str)
                    {
                        //TODO: validate string glyph
                    }
                    break;
                case GlyphKind.Image:
                    if (value is string imgPath)
                    {
                        //TODO: validate path of the glyph
                    }
                    //TODO: add supported formats for the glyph (bitmaps etc.)
                    break;
            }
            return false;
        }
    }

    public enum GlyphKind
    {
        /// <summary>
        /// The glyph is represented by the char symbol (e.g. /u0159)
        /// </summary>
        Char,
        /// <summary>
        /// The glyph is represented as string instruction
        /// </summary>
        String,
        /// <summary>
        /// The glyph is represented by a path to the existing image on device or in-memory image
        /// </summary>
        Image
    }
}