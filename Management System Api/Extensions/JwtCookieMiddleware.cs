using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Management_System_Api.Middleware
{
    public class JwtCookieMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _cookieName;

        public JwtCookieMiddleware(RequestDelegate next, string cookieName = "jwt")
        {
            _next = next;
            _cookieName = cookieName;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Cookies.ContainsKey(_cookieName))
            {
                var token = context.Request.Cookies[_cookieName];
                if (!string.IsNullOrEmpty(token))
                {
                    // Add token to Authorization header so JwtBearer can pick it up
                    context.Request.Headers["Authorization"] = $"Bearer {token}";
                }
            }

            await _next(context);
        }
    }

    // Extension method to add the middleware
    public static class JwtCookieMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtCookie(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtCookieMiddleware>();
        }
    }
}
