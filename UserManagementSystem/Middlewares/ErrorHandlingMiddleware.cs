namespace UserManagementSystem.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        //private readonly ILogger<ErrorHandlingMiddleware> _logger;

        //public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
            //_logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                //_logger.LogError(ex, "An error occurred during the request.");

                // Redirect to custom error page
                context.Response.StatusCode = 500;
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync($"<title>Error</title><div style='display: grid; place-items: center;margin-top:20vh;'><img src='https://cdn.dribbble.com/users/5360303/screenshots/19132462/media/88f1307d1a6b8fd41580f7f49d68ccea.jpg?compress=1&resize=400x300'><p>Error: {ex.Message}</p></div>");
            }
        }
    }
}
