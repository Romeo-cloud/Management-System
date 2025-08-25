using Management_System_Api.Data;
using Management_System_Api.Extensions;
using Management_System_Api.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .WriteTo.Console());

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o => SwaggerExtensions.ConfigureSwaggerJwt(o));

builder.Services.AddAppDb(builder.Configuration);
builder.Services.AddIdentityAndJwt(builder.Configuration);
builder.Services.AddAppServices();
builder.Services.AddMappingAndValidation();
builder.Services.AddCorsPolicy(builder.Configuration);
builder.Services.AddIpRateLimiting();

var app = builder.Build();


// Auto-migrate and seed roles/admin
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync(); // Replace with MigrateAsync for real migrations

    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // 1. Always ensure roles exist
    foreach (var r in new[] { "Admin", "Salesperson", "Operator" })
        if (!await roleMgr.RoleExistsAsync(r))
            await roleMgr.CreateAsync(new IdentityRole(r));

    // 2. Seed Admin only if credentials are available
    var adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL")
                     ?? app.Configuration["Seed:AdminEmail"];

    var adminPwd = Environment.GetEnvironmentVariable("ADMIN_PASSWORD")
                   ?? app.Configuration["Seed:AdminPassword"];

    if (!string.IsNullOrWhiteSpace(adminEmail) &&
        !string.IsNullOrWhiteSpace(adminPwd) &&
        await userMgr.FindByEmailAsync(adminEmail) is null)
    {
        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FullName = "System Admin",
            EmailConfirmed = true // optional: mark as verified
        };

        var result = await userMgr.CreateAsync(admin, adminPwd!);
        if (result.Succeeded)
        {
            await userMgr.AddToRoleAsync(admin, "Admin");
            Console.WriteLine($"Admin account created: {adminEmail}");
        }
        else
        {
            Console.WriteLine("Failed to create admin account: " +
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
    else
    {
        Console.WriteLine("No admin credentials found. Skipping admin creation.");
    }
}


// Middlewares
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseSecureHeaders();
app.UseCors("DefaultCors");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
