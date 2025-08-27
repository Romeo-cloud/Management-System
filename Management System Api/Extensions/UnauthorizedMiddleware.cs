using System.Net;
using System.Text.Json;

namespace Management_System_Api.Middlewares
{
    public class UnauthorizedMiddleware
    {
        private readonly RequestDelegate _next;

        public UnauthorizedMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            // If response is 401 or 403, replace with JSON payload
            if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized ||
                context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
            {
                // Reset response to JSON
                context.Response.ContentType = "application/json";

                var result = new
                {
                    success = false,
                    statusCode = context.Response.StatusCode,
                    message = context.Response.StatusCode == (int)HttpStatusCode.Unauthorized
                        ? "Unauthorized: Please login to access this resource."
                        : "Forbidden: You do not have permission to access this resource."
                };

                var json = JsonSerializer.Serialize(result);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
