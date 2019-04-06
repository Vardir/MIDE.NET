using System.Text.RegularExpressions;

namespace MIDE.API.Validations
{
    public class DefaultStringValidation : Validation<string>
    {
        public bool AllowNullOrEmpty { get; }
        public string RegexPattern { get; }

        public DefaultStringValidation(bool allowNullOrEmpty, string regex = null)
        {
            RegexPattern = regex;
            AllowNullOrEmpty = allowNullOrEmpty;
        }

        public override void Validate(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                if (!AllowNullOrEmpty)
                    AddError("Value can not be null or empty", value);
                return;
            }
            if (!string.IsNullOrEmpty(RegexPattern) && !Regex.IsMatch(value, RegexPattern))
                AddError("Value does not match a pattern", value);
        }
    }
}