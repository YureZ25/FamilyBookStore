using Data.Entities.Contracts;
using System.ComponentModel.DataAnnotations;

namespace Services.Extensions
{
    public static class ValidationExtensions
    {
        public static bool Validate<T>(this T entity, out IEnumerable<ValidationResult> errors) where T : class, IEntity
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(entity);

            var isValid = Validator.TryValidateObject(entity, validationContext, validationResults, validateAllProperties: true);

            errors = !isValid ? validationResults : null;

            return isValid;
        }
    }
}
