using System.ComponentModel.DataAnnotations;
using UserManagementSystem.Utils;

namespace UserManagementSystem.CustomAttributes
{
    public class StringRangeAttribute:ValidationAttribute
    {
        public int _minLength { get; }
        public int _maxLength { get; }

        public StringRangeAttribute(int MinLength = 0, int MaxLength = 0)
        {
            _minLength = MinLength;
            _maxLength = MaxLength;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string stringValue)
            {
                string propertyName = (validationContext.MemberName) ?? "Field";
                string simplifiedPropertyName = Auth.GetSimplifiedPropertyString(propertyName);
                if (stringValue.Length < _minLength)
                {
                    return new ValidationResult($"{simplifiedPropertyName} must be at least {_minLength} characters.");
                } else if (stringValue.Length > _maxLength)
                {
                    return new ValidationResult($"{simplifiedPropertyName} should not be longer than {_maxLength} characters.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
