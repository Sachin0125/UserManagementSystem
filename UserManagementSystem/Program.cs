using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Linq;
using UserManagementSystem.Models.User;
using System.Text.Json;
using NuGet.Common;
using UserManagementSystem.Middlewares;
namespace UserManagementSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            // for development environment
            builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
            string apiBaseUrl = builder.Configuration["ApiSettings:WebApiUrl"];
            builder.Services.AddHttpClient("API", client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            var app = builder.Build();
            // Configure the HTTP request pipeline.
            app.UseMiddleware<ErrorHandlingMiddleware>();
            if (!app.Environment.IsDevelopment())
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseExceptionHandler("/Home/Error");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseMiddleware<JwtMiddleware>();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=User}/{action=login}/{id?}");

            app.Run();
        }
    }
}
