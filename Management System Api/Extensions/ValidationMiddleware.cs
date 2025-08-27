using Microsoft.AspNetCore.Mvc;

namespace Management_System_Api.Middlewares
{
    public class ValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Run next
            await _next(context);

            // Check if ModelState is invalid
            if (context.Response.StatusCode == 400 && !context.Response.HasStarted)
            {
                var problemDetails = new ValidationProblemDetails
                {
                    Status = 400,
                    Title = "Validation failed",
                    Detail = "One or more validation errors occurred."
                };

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(problemDetails);
            }
        }
    }
}
