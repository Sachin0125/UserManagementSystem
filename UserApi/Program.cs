using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using UserApi.Data;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using UserApi.Constants;
using Serilog;


var secretKey = Environment.GetEnvironmentVariable("JWT__SALT");
var issuerAndAudience = Environment.GetEnvironmentVariable("raj");
//var issuerAndAudience = Environment.GetEnvironmentVariable("JWT__ISSUER");

if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuerAndAudience))
{
    throw new InvalidOperationException("JWT secret key or issuer/audience is not set.");
}


var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("Logs/appLog.txt",
        rollingInterval: RollingInterval.Day, // Create a new log file every day
        retainedFileCountLimit: 7)  // Keep logs for the last 7 days
    .CreateLogger();

// Replace the default logger with Serilog
builder.Host.UseSerilog();
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext
    >(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register API Versioning
builder.Services.AddApiVersioning(options =>
{
    // Specify that we want versioning based on the header (e.g., X-API-Version)
    options.ApiVersionReader = new HeaderApiVersionReader("X-API-Version");
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0); // Default to v1.0
});

// JWT Authentication
builder.Services.AddAuthentication(options => {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;  // Set to `true` in production for secure HTTPS connections
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuerAndAudience,
                ValidAudience = issuerAndAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };
        });

// Configure CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("Secured", policy =>
    {
        policy.WithOrigins("https://localhost:7122/")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("current_version", new OpenApiInfo { Title = "Users API", Version = AppConstants.CURRENT_VERSION, Description="<b>Current Version</b>" });
    c.SwaggerDoc("future_version", new OpenApiInfo { Title = "Users API", Version = AppConstants.FUTURE_VERSION, Description = "<b>Future Version</b>" });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/current_version/swagger.json", "Users API");
        c.SwaggerEndpoint("/swagger/future_version/swagger.json", "Users API");
    });
}
app.UseHttpsRedirection();
app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        context.Response.StatusCode = 400;
        context.Response.ContentType = "application/json";

        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (exceptionFeature != null)
        {
            var exception = exceptionFeature.Error;
            // Log and send a structured error response
            var errorResponse = new
            {
                Message = "An error occurred while processing your request.",
                Details = exception.Message
            };
            logger.LogError($"Exception Message: {errorResponse}");
            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    });
});
// Middleware configuration
app.UseRouting();

// Apply CORS policy
app.UseCors("Secured");

// Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map controller actions to routes
app.MapControllers();
app.Run();
