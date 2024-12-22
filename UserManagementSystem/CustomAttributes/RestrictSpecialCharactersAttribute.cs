using System.ComponentModel.DataAnnotations;
using UserManagementSystem.Constants;
using UserManagementSystem.Utils;

namespace UserManagementSystem.CustomAttributes
{
    public class RestrictSpecialCharactersAttribute : ValidationAttribute
    {
        private readonly string _allowedSpecialChars;
        public RestrictSpecialCharactersAttribute(string allowedSpecialChars) : base(AppConstants.SPECIAL_CHAR_ERROR_MSG)
        {
            _allowedSpecialChars = string.IsNullOrEmpty(allowedSpecialChars)? string.Empty : allowedSpecialChars;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            char[] specialChars = _allowedSpecialChars.ToCharArray();
            if (value != null && value.ToString().Any(ch => !(Auth.ValidateChars(ch, specialChars))))
            {
                return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }
}
