using System;
using System.Text.RegularExpressions;

namespace MIDE.Application.Attrubites
{
    public class ExtensionPropertiesAttribute : Attribute
    {
        private string version;

        public string Version
        {
            get => version;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException();
                if (!Regex.IsMatch(value, ApplicationPropertiesAttribute.VERSION_PATTERN))
                    throw new FormatException("Value was of invalid format");
                version = value;
            }
        }

        public ExtensionPropertiesAttribute() { }
    }
}