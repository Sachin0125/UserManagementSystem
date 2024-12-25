using System.ComponentModel.DataAnnotations;
using UserManagementSystem.CustomAttributes;

namespace UserManagementSystem.Models.User
{
    public class UserLoginDto
    {
        [Required]
        [RestrictSpecialCharacters(allowedSpecialChars: "@,.,-,_")]
        [StringRange(3, 50)]
        public string LoginId { get; set; }
        [Required]
        [StringRange(6, 20)]
        public string Password { get; set; }
    }
    public class TokenResponse
    {
        public string? token { get; set; }
    }
    public class UserRegistrationDto
    {
        [Required]
        [StringRange(3, 20)]
        [RestrictSpecialCharacters(allowedSpecialChars: "@,.,-,_")]
        public string UserName { get; set; }

        [Required]
        [StringRange(6, 20)]
        public string Password { get; set; }

        [Required]
        [StringRange(6, 20)]
        [Compare("Password", ErrorMessage="Password do not match.")]
        public string ConfirmPassword { get; set; }

        [StringRange(0, 50)]
        public string? FirstName { get; set; }

        [StringRange(0, 50)]
        public string? LastName { get; set; }

        [Required]
        [StringRange(6, 50)]
        [EmailAddress]
        public string Email { get; set; }
    }
    public class UserProfileDto
    {
        public string UserName { get; set; }

        [StringRange(0, 50)]
        public string? FirstName { get; set; }

        [StringRange(0, 50)]
        public string? LastName { get; set; }
        public string Email { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? createdAt { get; set; }
    }
}
