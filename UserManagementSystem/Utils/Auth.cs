using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using UserManagementSystem.Models.User;

namespace UserManagementSystem.Utils
{
    public class Auth
    {
        /// <summary>
        /// Get JWT token
        /// </summary>
        /// <param name="request">HttpRequest</param>
        /// <returns></returns>
        public static string? GetJwtToken(HttpRequest request) {
            var authToken = request.Cookies["jwt_token"];
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(authToken);
            return tokenResponse?.token;
        }

        /// <summary>
        /// Method to decode the JWT token and extract claims
        /// </summary>
        /// <param name="token">JWT Token</param>
        /// <returns></returns>
        public static IEnumerable<Claim> GetClaimsFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            // Return the claims from the JWT token
            return jsonToken?.Claims;
        }
        /// <summary>
        /// Return login page url
        /// </summary>
        /// <param name="context">HttpContext</param>
        public static void RedirectToLogin(HttpContext context) {
            context.Response.Cookies.Delete("jwt_token");
            context.Response.Redirect("/user/login");
        }

        /// <summary>
        /// Used to set JWT token in browser cookies
        /// </summary>
        /// <param name="response">HttpResponse</param>
        /// <param name="token">JWT Token</param>
        public static void SetTokenInCookie(HttpResponse response, string token)
        {
            // Store the JWT token in a cookie with a secure flag
            response.Cookies.Append("jwt_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,        // Set to true in production
                SameSite = SameSiteMode.Strict
            });
        }

        /// <summary>
        /// Verify whether input charactor is alphanumical or any allowed special charactor
        /// </summary>
        /// <param name="ch"> Input charactor</param>
        /// <param name="allowedSpecialChars">List of allowed special charactor</param>
        /// <returns></returns>
        public static bool ValidateChars(char ch, char[] allowedSpecialChars)
        {
            bool isLetterorDigit = char.IsLetterOrDigit(ch);
            bool isAllowedSpecialChar = allowedSpecialChars.Any(sChar => sChar == ch);
            return isLetterorDigit || isAllowedSpecialChar;
        }

        /// <summary>
        /// Get simplified property name
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetSimplifiedPropertyString(string input) {
            return input.ToLower() switch
            {
                "username" => "User name",
                "loginid" => "User name/Email",
                "firstname" => "First name",
                "lastname" => "Last name",
                "email" => "Email address",
                _ => input
            };
        }
    }
}
