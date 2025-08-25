using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Management_System_Api.Mapping;
using Management_System_Api.Services.Implementations;
using Management_System_Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Management_System_Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        // 🔹 Database registration
        public static IServiceCollection AddAppDb(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<Data.AppDbContext>(opt =>
                opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));
            return services;
        }

        // 🔹 Application services
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ISalesService, SalesService>();
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IReportsService, ReportsService>();
            return services;
        }

        // 🔹 AutoMapper + FluentValidation
        public static IServiceCollection AddMappingAndValidation(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => { }, typeof(AutoMapperProfile).Assembly);

            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<AutoMapperProfile>();

            return services;
        }

        // 🔹 CORS
        public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration config)
        {
            var allowed = config.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
            services.AddCors(opt =>
            {
                opt.AddPolicy("DefaultCors", p =>
                    p.WithOrigins(allowed).AllowAnyHeader().AllowAnyMethod().AllowCredentials());
            });
            return services;
        }
    }
}
