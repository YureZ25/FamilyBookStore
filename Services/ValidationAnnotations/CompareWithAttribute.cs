using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Services.ValidationAnnotations
{
    public enum CompareMethod
    {
        Equal = ExpressionType.Equal,
        NotEqual = ExpressionType.NotEqual,
        LessThan = ExpressionType.LessThan,
        LessThanOrEqual = ExpressionType.LessThanOrEqual,
        GreaterThan = ExpressionType.GreaterThan,
        GreaterThanOrEqual = ExpressionType.GreaterThanOrEqual,
    }

    public sealed class CompareWithAttribute : ValidationAttribute
    {
        public CompareMethod CompareMethod { get; private set; }
        public string OtherPropertyName { get; private set; }

        public CompareWithAttribute(CompareMethod compareMethod, string otherPropertyName)
        {
            CompareMethod = compareMethod;
            OtherPropertyName = otherPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var thisProperty = validationContext.ObjectType.GetProperty(validationContext.MemberName)
                ?? throw new ApplicationException($"{validationContext.MemberName} not found in {validationContext.ObjectType} class.");

            var otherProperty = validationContext.ObjectType.GetProperties().SingleOrDefault(p => p.Name == OtherPropertyName)
                ?? throw new ApplicationException($"{OtherPropertyName} not found in {validationContext.ObjectType} class. Properties for compare must be defined in same type.");

            if (thisProperty.PropertyType != otherProperty.PropertyType)
                throw new ApplicationException("Properties for compare must be same type.");

            var obj = Expression.ConvertChecked(Expression.Constant(validationContext.ObjectInstance), validationContext.ObjectType);
            var thisValue = Expression.Property(obj, validationContext.MemberName);
            var otherValue = Expression.Property(obj, OtherPropertyName);
            var comparison = Expression.MakeBinary((ExpressionType)CompareMethod, thisValue, otherValue);
            var lambda = Expression.Lambda<Func<bool>>(comparison);
            var func = lambda.Compile();

            if (value != null && !func())
            {
                return new ValidationResult(ErrorMessage ?? $"Значение свойства '{validationContext.MemberName}' не прошло проверку сравнением с '{OtherPropertyName}'", [validationContext.MemberName, OtherPropertyName]);
            }
            return ValidationResult.Success;
        }
    }
}
