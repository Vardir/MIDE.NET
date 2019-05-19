using System;
using MIDE.Application;
using System.Globalization;
using MIDE.Application.Events;
using System.Text.RegularExpressions;
using MIDE.Helpers;

namespace MIDE.API.Components
{
    /// <summary>
    /// The base class that should be implemented for all elements
    /// that are required to be identified and considered as the application components.
    /// </summary>
    public abstract class ApplicationComponent : IEventListener
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
        public string Id { get; private set; }
        /// <summary>
        /// Shortcut to access application kernel
        /// </summary>
        public AppKernel Kernel { get; private set; }

        public ApplicationComponent(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("The ID must not be empty");
            if (!Regex.IsMatch(id, ID_PATTERN))
                throw new FormatException($"The ID '{id}' has invalid format");

            Id = id;
            Kernel = AppKernel.Instance;
        }

        /// <summary>
        /// Receives and operates on events provided by broadcaster
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        public virtual void Receive(object sender, string message) { } 

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

        /// <summary>
        /// Transforms the given string to valid ID
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToSafeId(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return "id";
            string s = str.ToLower();
            s = s.Replace('_', '-');
            s = Regex.Replace(s, @"[^a-z0-9\-]", string.Empty);
            s = s.Trim();
            s = s.TrimStart('0'.To('9'));
            s = s.Trim('-');
            return s;
        }

        protected internal void GenerateNextId()
        {
            if (char.IsDigit(Id[Id.Length - 1]))
            {
                int firstDigit = Id.Length - 1;
                bool hasDash = false;
                int i = Id.Length - 1;
                int number = 1;
                for (; i > 0; i--)
                {
                    if (!char.IsDigit(Id[i]))
                    {
                        hasDash = Id[i] == '-';
                        break;
                    }
                }
                string newId = Id;
                if (hasDash)
                {
                    number = int.Parse(Id.Substring(i + 1, Id.Length - (i + 1))) + 1;
                    newId = Id.Remove(i);
                }
                newId = newId + '-' + number;
                Id = newId;
                return;
            }
            Id = Id + "-" + 1;
        }
    }
}