using System;
using System.Text.RegularExpressions;

using Vardirsoft.XApp.Helpers;

namespace Vardirsoft.XApp.Application.Attributes
{
    public class ApplicationPropertiesAttribute : Attribute
    {
        public const string APP_NAME_PATTERN = @"^[\w]+$";

        public string ApplicationName { get; }

        public ApplicationPropertiesAttribute(string applicationName)
        {
            Guard.EnsureNonEmpty(applicationName, typeof(ArgumentException));
            Guard.Ensure(Regex.IsMatch(applicationName, APP_NAME_PATTERN), typeof(FormatException), "Application name does not match the pattern");
            
            ApplicationName = applicationName;
        }
    }
}