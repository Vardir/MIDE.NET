using System;
using MIDE.API.Components;
using System.Text.RegularExpressions;

namespace MIDE.Application.Attrubites
{
    public class DependencyAttribute : Attribute
    {
        private string version;
        private string dependentOn;

        public string DependentOn
        {
            get => dependentOn;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException();
                if (!Regex.IsMatch(value, ApplicationPropertiesAttribute.APP_NAME_PATTERN))
                    throw new FormatException("Value was of invalid format");
                dependentOn = value;
            }
        }
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
        public DependencyType Type { get; set; }

        public DependencyAttribute() { }
    }

    public enum DependencyType
    {
        ApplicationKernel, Extension
    }
}