using System.Drawing;
using MIDE.FileSystem;
using System.Globalization;
using System.ComponentModel;
using System.Text.RegularExpressions;
using MIDE.Application;

namespace MIDE.API.Visuals
{
    /// <summary>
    /// Implement this interface to provide possibility to add graphical information your application components 
    /// </summary>
    public class Glyph : INotifyPropertyChanged
    {
        private object value;
        private Color alternateColor;

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
        public Color AlternateColor
        {
            get => alternateColor;
            set
            {
                if (value == alternateColor)
                    return;
                alternateColor = value;
                OnPropertyChanged(nameof(AlternateColor));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Glyph()
        {
            AlternateColor = Color.White;
        }
        public Glyph(string fontAwesomeIcon) : base()
        {
            Kind = GlyphKind.FontAwesome;
            Value = fontAwesomeIcon;
        }
        public Glyph(char unicodeSymbol) : base()
        {
            Kind = GlyphKind.UnicodeSymbol;
            Value = unicodeSymbol;
        }
        public Glyph(object value, GlyphKind kind = GlyphKind.ImagePath) : base()
        {
            Kind = kind;
            Value = value;
        }

        public static Glyph From(string format)
        {
            Glyph glyph = null;
            if (format == null)
                return null;
            if (format.Length > 4 && format.StartsWith("@fa-")) // @fa-f07c:0000ff -- blue folder icon
            {
                int delim = format.IndexOf(':');
                if (delim != -1 && format.Length > 8)
                {
                    char icon = (char)int.Parse(format.Substring(4, delim - 4), NumberStyles.AllowHexSpecifier);
                    string colorHex = format.Substring(delim + 1);
                    Color color = Color.FromArgb(int.Parse(colorHex, NumberStyles.AllowHexSpecifier));
                    glyph = new Glyph(icon, GlyphKind.FontAwesome)
                    {
                        AlternateColor = color
                    };
                }
                else
                    glyph = new Glyph(format.Substring(4));
            }
            else if (format.Length > 4 && format.StartsWith("@p-")) // @p-root/assets/icons/file.png -- path to image
            {
                glyph = new Glyph(format.Substring(3), GlyphKind.ImagePath);
            }
            else if (format.Length > 6 && format.StartsWith("@pba-")) // @pba-root/assets/icons/file.png -- path to image to load as byte array
            {
                string path = format.Substring(5);
                byte[] array = FileManager.TryReadBytes(path);
                if (array != null)
                    glyph = new Glyph(array, GlyphKind.ByteArray);
            }
            return glyph;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool Validate(object value)
        {
            switch (Kind)
            {
                case GlyphKind.UnicodeSymbol:
                    if (value is string code)
                    {
                        return !string.IsNullOrWhiteSpace(code) &&
                               Regex.IsMatch(code, @"^\p{L}+$");
                    }
                    return value is char;
                case GlyphKind.FontAwesome:
                    if (value is string str)
                    {
                        return !string.IsNullOrWhiteSpace(str) &&
                               Regex.IsMatch(str, "^f[0-9a-fA-f]{3}$");
                    }
                    return value is char;
                case GlyphKind.ImagePath:
                    if (value is string imgPath)
                    {
                        return !string.IsNullOrWhiteSpace(imgPath) &&
                                FileManager.Exists(imgPath);
                    }
                    break;
                case GlyphKind.ByteArray:
                    return value is byte[];
            }
            return false;
        }
    }

    public enum GlyphKind
    {
        /// <summary>
        /// The glyph is represented by the char symbol (e.g. /u0159)
        /// </summary>
        UnicodeSymbol,
        /// <summary>
        /// The glyph is represented as string instruction
        /// </summary>
        FontAwesome,
        /// <summary>
        /// The glyph is represented by a path to the existing image on device
        /// </summary>
        ImagePath,
        /// <summary>
        /// The glyph is represented by a byte array
        /// </summary>
        ByteArray
    }
}