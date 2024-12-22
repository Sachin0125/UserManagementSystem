using System.ComponentModel.DataAnnotations;

namespace UserManagementSystem.CustomAttributes {
    public class ComparePropertiesAttribute : ValidationAttribute
    {
        private readonly string _otherProperty;

        public ComparePropertiesAttribute(string otherProperty)
        {
            _otherProperty = otherProperty ?? throw new ArgumentNullException(nameof(otherProperty));
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var otherProperty = validationContext.ObjectType.GetProperty(_otherProperty);

            if (otherProperty == null)
            {
                return new ValidationResult($"Unknown property: {_otherProperty}");
            }

            var otherValue = otherProperty.GetValue(validationContext.ObjectInstance, null);

            if (!object.Equals(value, otherValue))
            {
                return new ValidationResult($"{validationContext.DisplayName} and {_otherProperty} don't match.");
            }

            return ValidationResult.Success;
        }
    }
}