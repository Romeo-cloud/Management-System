using System.Net;
using System.Text.Json;
using Management_System_Api.Exceptions;

namespace Management_System_Api.Extensions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;


        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        { _next = next; _logger = logger; }


        public async Task Invoke(HttpContext ctx)
        {
            try { await _next(ctx); }
            catch (ApiException ex)
            {
                _logger.LogWarning(ex, "Handled API error");
                await WriteAsync(ctx, ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error");
                await WriteAsync(ctx, (int)HttpStatusCode.InternalServerError, "An unexpected error occurred.");
            }
        }


        private static Task WriteAsync(HttpContext ctx, int status, string message)
        {
            ctx.Response.StatusCode = status;
            ctx.Response.ContentType = "application/json";
            var body = JsonSerializer.Serialize(new { error = message, status });
            return ctx.Response.WriteAsync(body);
        }
    }
}
