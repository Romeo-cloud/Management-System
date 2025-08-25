namespace Management_System_Api.Extensions
{
    public static class SecurityHeadersExtensions
    {
        public static IApplicationBuilder UseSecureHeaders(this IApplicationBuilder app)
        {
            return app.Use(async (ctx, next) =>
            {
                var headers = ctx.Response.Headers;
                headers["X-Content-Type-Options"] = "nosniff";
                headers["X-Frame-Options"] = "DENY";
                headers["Referrer-Policy"] = "no-referrer";
                headers["X-XSS-Protection"] = "0";
                headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";
                await next();
            });
        }
    }
}
