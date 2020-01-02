using System.Drawing;
using System.Globalization;
using System.ComponentModel;
using System.Text.RegularExpressions;

using Vardirsoft.Shared.Helpers;

using Vardirsoft.XApp.IoC;
using Vardirsoft.XApp.API;

namespace Vardirsoft.XApp.Visuals
{
    /// <summary>
    /// Implement this interface to provide possibility to add graphical information your application components 
    /// </summary>
    public class Glyph : INotifyPropertyChanged
    {
        private object _value;
        private Color _alternateColor;

        public object Value
        {
            get => _value;
            set
            {
                if (_value == value)
                    return;

                if (Validate(value))
                {
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }
        public GlyphKind Kind { get; }
        public Color AlternateColor
        {
            get => _alternateColor;
            set
            {
                if (value == _alternateColor)
                    return;

                _alternateColor = value;
                OnPropertyChanged(nameof(AlternateColor));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Glyph()
        {
            AlternateColor = Color.White;
        }
        public Glyph(string fontAwesomeIcon)
        {
            Kind = GlyphKind.FontAwesome;
            Value = fontAwesomeIcon;
        }
        public Glyph(char unicodeSymbol)
        {
            Kind = GlyphKind.UnicodeSymbol;
            Value = unicodeSymbol;
        }
        public Glyph(object value, GlyphKind kind = GlyphKind.ImagePath)
        {
            Kind = kind;
            Value = value;
        }

        public static Glyph From(string format)
        {
            Glyph glyph = null;
            if (format is null)
                return null;

            if (format.Length > 4 && format.StartsWith("@fa-")) // @fa-f07c:0000ff -- blue folder icon
            {
                var delim = format.IndexOf(':');
                if (delim != -1 && format.Length > 8)
                {
                    var icon = (char)int.Parse(format.Substring(4, delim - 4), NumberStyles.AllowHexSpecifier);
                    var colorHex = format.Substring(delim + 1);
                    var color = Color.FromArgb(int.Parse(colorHex, NumberStyles.AllowHexSpecifier));

                    glyph = new Glyph(icon, GlyphKind.FontAwesome)
                    {
                        AlternateColor = color
                    };
                }
                else
                {
                    glyph = new Glyph(format.Substring(4));
                }
            }
            else if (format.Length > 4 && format.StartsWith("@p-")) // @p-root/assets/icons/file.png -- path to image
            {
                glyph = new Glyph(format.Substring(3), GlyphKind.ImagePath);
            }
            else if (format.Length > 6 && format.StartsWith("@pba-")) // @pba-root/assets/icons/file.png -- path to image to load as byte array
            {
                var path = format.Substring(5);
                var array = IoCContainer.Resolve<IFileManager>().TryReadBytes(path);

                if (array.HasValue())
                {    
                    glyph = new Glyph(array, GlyphKind.ByteArray);
                }
            }
            return glyph;
        }

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private bool Validate(object value)
        {
            switch (Kind)
            {
                case GlyphKind.UnicodeSymbol:
                    if (value is string code)
                    {
                        return code.HasValue() && Regex.IsMatch(code, @"^\p{L}+$");
                    }
                    return value is char;
                case GlyphKind.FontAwesome:
                    if (value is string str)
                    {
                        return str.HasValue() && Regex.IsMatch(str, "^f[0-9a-fA-f]{3}$");
                    }
                    return value is char;
                case GlyphKind.ImagePath:
                    if (value is string imgPath)
                    {
                        return imgPath.HasValue() && IoCContainer.Resolve<IFileManager>().Exists(imgPath);
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