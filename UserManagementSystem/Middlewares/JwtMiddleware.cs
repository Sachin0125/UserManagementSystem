using Microsoft.AspNetCore.Mvc.Routing;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using UserManagementSystem.Constants;
using UserManagementSystem.Models.User;
using UserManagementSystem.Utils;

namespace UserManagementSystem.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var jwtCookies = context?.Request?.Cookies["jwt_token"];
            if (string.IsNullOrEmpty(jwtCookies))
            {
                var currentPath = context?.Request?.Path.Value.ToLower();
                if (!AppConstants.ANONYMOUS_PAGES.Any(page => currentPath.Contains(page.ToLower())))
                {
                    context?.Response.Redirect("/User/login");
                    return;
                }
            }
            else
            {
                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(jwtCookies);
                if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.token))
                {
                    Auth.RedirectToLogin(context);
                    return;
                }
                string jwtToken = tokenResponse.token;
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(jwtToken) as JwtSecurityToken;

                var expirationDate = jsonToken?.ValidTo;
                if (expirationDate < DateTime.UtcNow)
                {
                    Auth.RedirectToLogin(context);
                    return;
                }

                // Return the claims from the JWT token
                var claims = jsonToken?.Claims;

                if (claims != null && claims.Any())
                {
                    // Create a ClaimsPrincipal from the claims
                    var identity = new ClaimsIdentity(claims, "jwt");
                    var claimsPrincipal = new ClaimsPrincipal(identity);

                    // Set the ClaimsPrincipal in the current HTTP context
                    context.User = claimsPrincipal;
                }
            }
            await _next(context);
        }
    }
}
