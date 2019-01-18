﻿using System.Drawing;
using System.ComponentModel;

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
                        //TODO: validate Unicode symbol format
                    }
                    return value is char;
                case GlyphKind.FontAwesome:
                    if (value is string str)
                    {
                        //TODO: add format validation
                        return !string.IsNullOrWhiteSpace(str);
                    }
                    return value is char;
                case GlyphKind.ImagePath:
                    if (value is string imgPath)
                    {
                        //TODO: validate path of the glyph
                    }
                    break;
                case GlyphKind.Bitmap:
                    //TODO: validate reference to a bitmap
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
        /// The glyph is represented by an in-memory bitmap image
        /// </summary>
        Bitmap
    }
}