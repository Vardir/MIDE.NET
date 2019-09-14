using System;
using System.Text.RegularExpressions;

namespace XApp.Application.Attributes
{
    public class ApplicationPropertiesAttribute : Attribute
    {
        public const string APP_NAME_PATTERN = @"^[\w]+$";

        public string ApplicationName { get; }

        public ApplicationPropertiesAttribute(string applicationName)
        {
            if (string.IsNullOrWhiteSpace(applicationName))
                throw new ArgumentException("Attribute must not be null or empty", nameof(applicationName));

            if (Regex.IsMatch(applicationName, APP_NAME_PATTERN))
            {
                ApplicationName = applicationName;
                
                return;
            }
            
            throw new FormatException("Application name does not match the pattern");
        }
    }
}