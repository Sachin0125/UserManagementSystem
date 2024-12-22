using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using UserManagementSystem.Models.User;
using System.Security.Claims;
using UserManagementSystem.Utils;
using UserManagementSystem.Constants;

namespace UserManagementSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        public UserController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        /// <summary>
        /// Registration GET (Registration Form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Registration POST (Submit Registration)
        /// </summary>
        /// <param name="User Registration model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegistrationDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = GetHttpClient();
            var response = await client.PostAsJsonAsync(AppConstants.REGISTER_URL, model);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", AppConstants.REGISTRATION_ERROR_MSG);
            return View(model);
        }

        /// <summary>
        /// Login GET (Login Form)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Login()
        {
            var token = Request.Cookies["jwt_token"];
            if (!string.IsNullOrEmpty(token))
            {
                if(!string.IsNullOrEmpty(Auth.GetJwtToken(Request)))
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            return View();
        }

        /// <summary>
        /// Login POST (Submit Login)
        /// </summary>
        /// <param name="Login model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = GetHttpClient();
            var response = await client.PostAsJsonAsync(AppConstants.LOGIN_URL, model);
            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
                // Store the token in a cookie or local storage
                Auth.SetTokenInCookie(Response, token);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", AppConstants.INVALID_CREDS_MSG);
            return View(model);
        }

        /// <summary>
        /// Profile GET (View Profile)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            HttpClient client = GetHttpClient(true);
            var response = await client.GetFromJsonAsync<UserProfileDto>(AppConstants.PROFILE_URL);

            if (response != null)
            {
                return View(response);
            }

            return RedirectToAction("Login");

        }

        /// <summary>
        /// Profile Post (Submit Profile)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(UserProfileDto model)
        {
            if (!ModelState.IsValid)
                return View(model);
            HttpClient client = GetHttpClient(true);
            var response = await client.PostAsJsonAsync(AppConstants.PROFILE_URL, model);

            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
                // Store the token in a cookie or local storage
                Auth.SetTokenInCookie(Response, token);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", AppConstants.PROFILE_UPDATE_ERROR_MSG);
            return View(model);
        }

        /// <summary>
        /// Logout - Delete JWT_Token and clear the httpcontext.user, then redirect to login page
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt_token");
            HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());
            return RedirectToAction("login");
        }

        /// <summary>
        /// Get HttpClient from IHttpFactory
        /// </summary>
        /// <param name="ifToken"></param>
        /// <returns></returns>
        private HttpClient GetHttpClient(bool ifToken = false) {
            var client = _clientFactory.CreateClient("API");
            if (ifToken) {
                string token = Auth.GetJwtToken(Request);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }            
            return client;
        }
    }
}
