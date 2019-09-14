using System.Collections.Generic;
using System.Text.RegularExpressions;

using XApp.Helpers;

namespace XApp.API.Validations
{
    public class DefaultStringValidation : ValueValidation<string>
    {
        public bool AllowNullOrEmpty { get; set; }
        public string RegexPattern { get; set; }
        
        public DefaultStringValidation(){}
        public DefaultStringValidation(bool allowNullOrEmpty, string regexPattern)
        {
            RegexPattern = regexPattern;
            AllowNullOrEmpty = allowNullOrEmpty;
        }

        protected override IEnumerable<(string, object)> Validate(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                if (!AllowNullOrEmpty)
                    yield return ("Value can not be null or empty", value);

                yield break;
            }

            if (RegexPattern.HasValue() && !Regex.IsMatch(value, RegexPattern))
                yield return ("Value does not match a pattern", value);
        }
    }
}