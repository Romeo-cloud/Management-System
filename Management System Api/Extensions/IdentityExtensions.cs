using Management_System_Api.Data;
using Management_System_Api.Models.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Management_System_Api.Extensions
{
    public static class IdentityExtensions
    {
        public static IServiceCollection AddIdentityAndJwt(this IServiceCollection services, IConfiguration config)
        {
            // ==================== Identity Setup ====================
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Password rules
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 12;
                options.Password.RequiredUniqueChars = 3;

                // Lockout rules
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User rules
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // ==================== JWT Setup ====================
            var jwtKey = Environment.GetEnvironmentVariable("JWT__KEY") ?? config["Jwt:Key"];
            var jwtIssuer = Environment.GetEnvironmentVariable("JWT__ISSUER") ?? config["Jwt:Issuer"];
            var jwtAudience = Environment.GetEnvironmentVariable("JWT__AUDIENCE") ?? config["Jwt:Audience"];

            if (string.IsNullOrEmpty(jwtKey))
                throw new InvalidOperationException("JWT Key is not configured. Please set JWT__KEY in environment variables or appsettings.json.");

            var key = Encoding.UTF8.GetBytes(jwtKey);

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };
            });

            // ==================== Authorization Policies ====================
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdmin", policy =>
                    policy.RequireRole("Admin"));

                options.AddPolicy("RequireOperator", policy =>
                    policy.RequireRole("Operator"));

                options.AddPolicy("RequireSales", policy =>
                    policy.RequireRole("Salesperson"));
            });

            return services;
        }

        // ==================== Seed Roles at startup ====================
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = { "Admin", "Operator", "Salesperson" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
