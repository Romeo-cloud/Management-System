using Management_System_Api.Data;
using Management_System_Api.Extensions;
using Management_System_Api.Models.Domain;
using Management_System_Api.Services;
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
    await db.Database.EnsureCreatedAsync(); // ⚠️ Replace with db.Database.MigrateAsync() in production

    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Ensure roles exist
    foreach (var r in new[] { "Admin", "Salesperson", "Operator" })
    {
        if (!await roleMgr.RoleExistsAsync(r))
            await roleMgr.CreateAsync(new IdentityRole(r));
    }

    // ✅ Get admin credentials (env overrides config)
    var adminEmail = app.Configuration["Seed:AdminEmail"];
    var adminPwd = app.Configuration["Seed:AdminPassword"];

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
            Console.WriteLine($"✅ Admin account created: {adminEmail}");
        }
        else
        {
            Console.WriteLine(" Failed to create admin account: " +
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
    else
    {
        Console.WriteLine(" No admin credentials found. Skipping admin creation.");
    }
}

// ==================== Middleware Pipeline ====================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseRequestLogging();          // Custom logging middleware
app.UseApiExceptionHandler();     // Global exception handling
app.UseValidationHandler();       // Model validation errors -> JSON

app.UseHttpsRedirection();
app.UseSecureHeaders();
app.UseCors("DefaultCors");
app.UseRateLimiter();

app.UseAuthentication();
app.UseUnauthorizedHandler();     // Catch 401 & 403 -> JSON
app.UseAuthorization();

app.MapControllers();
app.Run();
