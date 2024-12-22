using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using UserManagementSystem.Models.User;
using UserManagementSystem.test.Models;
//using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace UserManagementSystem.test.UnitTests
{
    public class Login
    {
        private readonly IConfiguration _configuration;

        public Login()
        {
            // TODO
            // Setup configuration from appsettings.Development.json
            //_configuration = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory()) // Set base path for the configuration
            //    .AddJsonFile("../../UserManagementSystem/appsettings.Development.json", optional: true, reloadOnChange: true)
            //    .Build();            
            //var webAPIUrl = _configuration.GetValue<string>("ApiSettings:WebApiUrl");
        }
        [Fact]
        public async Task ValidateLoginFunctionality()
        {
            // Create mock client and prepare data for request and response
            var mockHttp = new MockHttpMessageHandler();
            var mockResponse = JsonConvert.SerializeObject(new TokenResponse
            {
                token = Constants.jwtTokenFormat,
            });

            loginTestDto requestPayload = new loginTestDto
            {
              loginId = Constants.testLoginId,
              password =  Constants.testPassword,
            };

            var requestJson = JsonConvert.SerializeObject(requestPayload);

            //Setting up the Client
            mockHttp.When(HttpMethod.Post, Constants.loginEndPoint)
                    .WithContent(requestJson)
                    .Respond("application/json", mockResponse);
            var apiService = mockHttp.ToHttpClient();


            // Send Post requst and receive the response
            var response = await apiService.PostAsJsonAsync(Constants.loginEndPoint, requestPayload);
            var token = await response.Content.ReadAsStringAsync();

            //Assert the api response
            Assert.NotNull(token);
            var tokenParts = token?.Split(':')[1].Split(".");
            Assert.Equal(3, tokenParts?.Length); // JWT should have 3 parts
            Assert.True(token?.Length > 20);
        }
    }
}