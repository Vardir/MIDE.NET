using MIDE.Application;

namespace MIDE.API.Validations
{
    public class PathAttachedValidation : PropertyAttachedValidation<string>
    {
        public PathAttachedValidation(bool raiseExceptionOnError)
            : base(raiseExceptionOnError)
        {
            
        }

        protected override void Validate(string propertyName, string value)
        {
            ClearErrors();
            if (string.IsNullOrEmpty(value))
            {
                AddError(propertyName, "Path can not be null or empty", value);
                return;
            }
            if (!AppKernel.Instance.FileManager.Exists(value))
                AddError(propertyName, "Path does not exist", value);
        }
    }
}