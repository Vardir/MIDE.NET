using System.Collections;
using System.Collections.Generic;

namespace MIDE.API.Validations
{
    public abstract class ValueValidation<T> : Validation
    {
        /// <summary>
        /// Not implemented in this type of validation
        /// </summary>
        public override bool HasErrors => throw new System.NotImplementedException();

        /// <summary>
        /// Not implemented in this type of validation
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public override IEnumerable GetErrors(string propertyName)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Validates the given value and adds validation errors in the provided collection
        /// </summary>
        /// <param name="value"></param>
        /// <param name="errorsCollection"></param>
        public void Validate(T value, string propertyName, ICollection<ValidationError> errorsCollection)
        {
            foreach (var (message, invalidValue) in Validate(value))
                errorsCollection.Add(new ValidationError(propertyName, message, invalidValue));
        }

        protected abstract IEnumerable<(string message, object invalidValue)> Validate(T value);
    }
}