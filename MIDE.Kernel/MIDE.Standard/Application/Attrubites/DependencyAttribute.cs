using System;
using MIDE.API.Components;
using System.Text.RegularExpressions;

namespace MIDE.Application.Attrubites
{
    public class DependencyAttribute : Attribute
    {
        private string version;
        private string dependentOn;

        public const string VERSION_PATTERN = @"^[0-9]\.[0-9]\.[0-9]$";

        public string DependentOn
        {
            get => dependentOn;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException();
                if (!Regex.IsMatch(value, ApplicationComponent.ID_PATTERN))
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
                if (!Regex.IsMatch(value, VERSION_PATTERN))
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