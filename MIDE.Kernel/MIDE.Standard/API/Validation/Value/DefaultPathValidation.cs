using MIDE.FileSystem;
using System.Collections.Generic;

namespace MIDE.API.Validations
{
    public class DefaultPathValidation : ValueValidation<string>
    {
        public DefaultPathValidation()
        {

        }

        protected override IEnumerable<(string, object)> Validate(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                yield return ("Path can not be null or empty", value);
                yield break;
            }
            if (!FileManager.Instance.Exists(value))
                yield return ("Path does not exist", value);
        }
    }
}