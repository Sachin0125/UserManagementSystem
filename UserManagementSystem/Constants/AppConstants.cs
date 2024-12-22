using UserManagementSystem.Models.User;

namespace UserManagementSystem.Constants
{
    public static class AppConstants
    {
        public const string Login = "login";
        public const string Register = "register";
        public const string Profile = "profile";
        public const string Error = "error";

        public static readonly string[] ANONYMOUS_PAGES = { Login, Register, Error };
        private const string URL_PREFIX = "/api/user/";
        public const string LOGIN_URL = $"{URL_PREFIX}{Login}";
        public const string PROFILE_URL = $"{URL_PREFIX}{Profile}";
        public const string REGISTER_URL = $"{URL_PREFIX}{Register}";
        public const string REGISTRATION_ERROR_MSG = "Registration failed, please try again.";
        public const string INVALID_CREDS_MSG = "Invalid credentials, please try again.";
        public const string PROFILE_UPDATE_ERROR_MSG = "Profile update failed, please try again.";
        public const string SPECIAL_CHAR_ERROR_MSG = "Special characters are not allowed (expect: @,.,-,_)";
    }
}
