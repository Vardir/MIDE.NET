using System;
using System.Globalization;
using System.ComponentModel;
using MIDE.Standard.Helpers;
using System.Text.RegularExpressions;
using MIDE.Standard.API.Measurements;

namespace MIDE.Standard.API.Components
{
    public abstract class LayoutComponent : IApplicationComponent, INotifyPropertyChanged
    {
        protected readonly TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

        private bool isEnabled = true;
        private GridLength width;
        private GridLength height;
        private BoundingBox margin;
        private BoundingBox padding;
        private LayoutComponent parent;

        public const string ID_PATTERN = @"^([a-z]+[a-z0-9\-]*[a-z0-9]+)$";
        public const string ID_PATTERN_CLEAN = @"[a-z]+[a-z0-9\-]*[a-z0-9]+";

        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                if (isEnabled == value)
                    return;
                isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }
        public string Id { get; }
        public virtual GridLength Width
        {
            get => width;
            set
            {
                if (width == value)
                    return;
                width = value;
                OnPropertyChanged(nameof(Width));
            }
        }
        public virtual GridLength Height
        {
            get => height;
            set
            {
                if (height == value)
                    return;
                height = value;
                OnPropertyChanged(nameof(Height));
            }
        }
        public virtual BoundingBox Margin
        {
            get => margin;
            set
            {
                if (margin == value)
                    return;
                margin = value;
                OnPropertyChanged(nameof(Margin));
            }
        }
        public virtual BoundingBox Padding
        {
            get => padding;
            set
            {
                if (padding == value)
                    return;
                padding = value;
                OnPropertyChanged(nameof(Padding));
            }
        }
        public LayoutComponent Parent
        {
            get => parent;
            set
            {
                if (parent == value)
                    return;
                parent = value;
                OnPropertyChanged(nameof(Parent));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public LayoutComponent(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("The ID must not be empty");
            if (!Regex.IsMatch(id, ID_PATTERN))
                throw new FormatException($"The ID '{id}' has invalid format");
            Id = id;
            width = GridLength.Auto;
            height = GridLength.Auto;
        }
        
        public override string ToString() => this.GetSpec();

        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}