using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MIDE.API.Components
{
    /// <summary>
    /// The base class that should be implemented for all elements
    /// that are required to be identified and considered as the application components.
    /// </summary>
    public abstract class ApplicationComponent
    {
        private static readonly TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

        /// <summary>
        /// The RegEx pattern that is applied to ID of all the components
        /// </summary>
        public const string ID_PATTERN = @"^([a-z]+[a-z0-9\-]*[a-z0-9]+)$";
        /// <summary>
        /// The inline version of <seealso cref="ID_PATTERN"/> that can be used as single part in other patterns
        /// </summary>
        public const string ID_PATTERN_INL = @"[a-z]+[a-z0-9\-]*[a-z0-9]+";

        /// <summary>
        /// The ID of the component. It has uniform format for all the components in application
        /// </summary>
        public string Id { get; }

        public ApplicationComponent(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("The ID must not be empty");
            if (!Regex.IsMatch(id, ID_PATTERN))
                throw new FormatException($"The ID '{id}' has invalid format");

            Id = id;
        }

        /// <summary>
        /// Formats the ID of the component. Replace all '-' symbols with spaces and make the resulting sentence title case.
        /// <para>The ID 'hello-world' transforms into 'Hello World'</para>
        /// </summary>
        /// <returns></returns>
        public string FormatId() => textInfo.ToTitleCase(Id.Replace('-', ' '));
        /// <summary>
        /// Gets string specification representation of the component in the following format: [type] ID:[id]
        /// </summary>
        /// <returns></returns>
        public string GetSpec() => $"[{GetType().Name}] ID:{Id}";
        public override string ToString() => GetSpec();
    }
}