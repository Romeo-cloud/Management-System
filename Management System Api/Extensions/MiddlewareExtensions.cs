using Management_System_Api.Middlewares;

namespace Management_System_Api.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseApiExceptionHandler(this IApplicationBuilder app)
            => app.UseMiddleware<ExceptionMiddleware>();

        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
            => app.UseMiddleware<RequestLoggingMiddleware>();

        public static IApplicationBuilder UseValidationHandler(this IApplicationBuilder app)
            => app.UseMiddleware<ValidationMiddleware>();

        public static IApplicationBuilder UseUnauthorizedHandler(this IApplicationBuilder app)
            => app.UseMiddleware<UnauthorizedMiddleware>();
    }
}
