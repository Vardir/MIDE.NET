namespace MIDE.API.Validations
{
    public struct ValidationError
    {
        public readonly object InvalidValue;
        public readonly string PropertyName;
        public readonly string ValidationMessage;

        public ValidationError(string propertyName, string validationMessage, object invalidValue)
        {
            InvalidValue = invalidValue;
            PropertyName = propertyName;
            ValidationMessage = validationMessage;
        }

        public override string ToString() => ValidationMessage;
    }
}