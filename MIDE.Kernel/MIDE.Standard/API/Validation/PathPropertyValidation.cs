namespace MIDE.API.Validations
{
    public class PathPropertyValidation : PropertyValidation<string>
    {
        public PathPropertyValidation(bool raiseExceptionOnError)
            : base(raiseExceptionOnError)
        {
            
        }

        protected override void Validate(string propertyName, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                AddError(propertyName, "Path can not be null or empty", value);
                return;
            }
            if (!Application.AppKernel.Instance.FileManager.Exists(value))
                AddError(propertyName, "Path does not exist", value);
        }
    }
}