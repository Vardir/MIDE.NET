using System.Text.RegularExpressions;

namespace MIDE.API.Validations
{
    public class DefaultStringPropertyValidation : PropertyValidation<string>
    {
        public bool AllowNullOrEmpty { get; }
        public string RegexPattern { get; }

        public DefaultStringPropertyValidation(bool raiseExceptionOnError, bool allowNullOrEmpty, string regex = null) 
            : base(raiseExceptionOnError)
        {
            RegexPattern = regex;
            AllowNullOrEmpty = allowNullOrEmpty;
        }

        protected override void Validate(string propertyName, string value)
        {
            ClearErrors();
            if (string.IsNullOrEmpty(value))
            {
                if (!AllowNullOrEmpty)
                    AddError(propertyName, "Value can not be null or empty", value);
                return;
            }
            if (!string.IsNullOrEmpty(RegexPattern) && !Regex.IsMatch(value, RegexPattern))
                AddError(propertyName, "Value does not match a pattern", value);
        }
    }
}