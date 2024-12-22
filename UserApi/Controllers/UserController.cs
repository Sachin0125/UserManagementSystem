using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserApi.Constants;
using UserApi.Data;
using UserApi.Modals;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace UserApi.Controllers
{
    [ApiVersion(AppConstants.CURRENT_VERSION)]
    [ApiVersion(AppConstants.FUTURE_VERSION)]
    public class UserController : BaseController
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
        }

        /// <summary>
        /// Api used to register new users
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationDTO model)
        {
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                return BadRequest("Email is already taken.");

            if (await _context.Users.AnyAsync(u => u.UserName == model.UserName))
                return BadRequest("UserName is already taken.");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                PasswordHash = hashedPassword,
                FirstName = model.FirstName ?? string.Empty,
                LastName = model.LastName ?? string.Empty,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow

            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully." });
        }

        /// <summary>
        /// Login user and return the JWT token if successful
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO model)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => (u.UserName == model.LoginId) || (u.Email == model.LoginId));
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials.");

            // Generate JWT token
            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        /// <summary>
        /// Get User profile data
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("Profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound("User not found.");

            var userProfile = new UserProfileDTO
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UpdatedAt = user.UpdatedAt,
                CreatedAt = user.CreatedAt,
                Email = user.Email,
            };

            return Ok(userProfile);
        }

        /// <summary>
        /// Update User profile
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("Profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileDTO model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _context.Users.FindAsync(userId);

            if (user == null) return NotFound("User not found.");

            user.FirstName = model.FirstName ?? string.Empty;
            user.LastName = model.LastName ?? string.Empty;
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // Generate JWT token
            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        /// <summary>
        /// Used to generate the JWT token using USer data
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private string GenerateJwtToken(User user)
        {
            // Retrieve the secret key and issuer/audience from environment variables
            var secretKey = Environment.GetEnvironmentVariable("JWT__SALT");
            var issuerAndAudience = Environment.GetEnvironmentVariable("raj");

            if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuerAndAudience))
            {
                throw new InvalidOperationException("JWT secret key or issuer/audience is not set.");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.GivenName, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuerAndAudience, // Set issuer to the environment variable value
                audience: issuerAndAudience, // Set audience to the same value
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
