using Management_System_Api.Data;
using Management_System_Api.Extensions;
using Management_System_Api.Models.Domain;
using Management_System_Api.Services;
using Management_System_Api.Middleware; // JWT cookie middleware
using Microsoft.AspNetCore.Identity;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ==================== Serilog ====================
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .WriteTo.Console());

// ==================== Services ====================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o => SwaggerExtensions.ConfigureSwaggerJwt(o));

builder.Services.AddAppDb(builder.Configuration);

// Add Identity + JWT Bearer authentication (supports Authorization header)
builder.Services.AddIdentityAndJwt(builder.Configuration);

builder.Services.AddAppServices();
builder.Services.AddMappingAndValidation();
builder.Services.AddCorsPolicy(builder.Configuration);
builder.Services.AddIpRateLimiting();
builder.Services.AddScoped<CreditService>();

var app = builder.Build();

// ==================== Auto-migrate & Seed Roles/Admin ====================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();

    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    foreach (var r in new[] { "Admin", "Salesperson", "Operator" })
    {
        if (!await roleMgr.RoleExistsAsync(r))
            await roleMgr.CreateAsync(new IdentityRole(r));
    }

    var adminEmail = builder.Configuration["Seed:AdminEmail"];
    var adminPwd = builder.Configuration["Seed:AdminPassword"];

    if (!string.IsNullOrWhiteSpace(adminEmail) &&
        !string.IsNullOrWhiteSpace(adminPwd) &&
        await userMgr.FindByEmailAsync(adminEmail) is null)
    {
        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FullName = "System Admin",
            EmailConfirmed = true
        };

        var result = await userMgr.CreateAsync(admin, adminPwd!);
        if (result.Succeeded)
        {
            await userMgr.AddToRoleAsync(admin, "Admin");
            Console.WriteLine($"Admin account created: {adminEmail}");
        }
        else
        {
            Console.WriteLine(" Failed to create admin account: " +
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}

// ==================== Middleware Pipeline ====================
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Management System API v1");
    c.RoutePrefix = "swagger";
    c.ConfigObject.AdditionalItems["withCredentials"] = true; // allow cookies
});

app.UseSerilogRequestLogging();
app.UseRequestLogging();
app.UseApiExceptionHandler();
app.UseValidationHandler();

app.UseHttpsRedirection();
app.UseSecureHeaders();
app.UseCors("DefaultCors");
app.UseRateLimiter();

// ==================== Authentication ====================

// 1️⃣ Cookie JWT middleware (Swagger/browser)
app.UseJwtCookie();

// 2️⃣ Standard JWT Bearer (Authorization header for CLI/Postman)
app.UseAuthentication();

app.UseUnauthorizedHandler();
app.UseAuthorization();

app.MapControllers();

app.Run();
