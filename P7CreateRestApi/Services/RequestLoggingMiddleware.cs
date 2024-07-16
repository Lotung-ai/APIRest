namespace P7CreateRestApi.Services
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation("Incoming request: {Method} {Path}", context.Request.Method, context.Request.Path);

            // Log the user identity if available
            if (context.User.Identity.IsAuthenticated)
            {
                _logger.LogInformation("User '{UserName}' is making a request.", context.User.Identity.Name);
            }
            else
            {
                _logger.LogWarning("Unauthorized access attempt to {Path}", context.Request.Path);
            }

            await _next(context);

            _logger.LogInformation("Request completed with status code {StatusCode}", context.Response.StatusCode);
        }
    }
}
