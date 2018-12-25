namespace MIDE.API.Validation
{
    public class TestValidator : PropertyValidator<string>
    {
        public TestValidator(bool raiseExceptionOnError) : base(raiseExceptionOnError) {}

        protected override void Validate(string propertyName, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                AddError(propertyName, "The value is null or empty", value);
                return;
            }
            if (value.Length > 24)
                AddError(propertyName, "The maximum length of the string is 24", value);
            if (value.Contains("/"))
                AddError(propertyName, "The string should not contain forward slashes ('/')", value);
        }
    }
}