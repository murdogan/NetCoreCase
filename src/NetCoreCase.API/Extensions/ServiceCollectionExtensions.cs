using Microsoft.EntityFrameworkCore;
using NetCoreCase.Application.Interfaces;
using NetCoreCase.Application.Services;
using NetCoreCase.Domain.Interfaces;
using NetCoreCase.Infrastructure.Data;
using NetCoreCase.Infrastructure.Services;

namespace NetCoreCase.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Application Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IContentService, ContentService>();
        services.AddScoped<ICacheService, CacheService>();

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database Context - PostgreSQL
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Memory Cache
        services.AddMemoryCache();

        return services;
    }

    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        // Add Controllers
        services.AddControllers();

        // API Explorer for Swagger
        services.AddEndpointsApiExplorer();

        // Swagger/OpenAPI
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "NetCore CMS API",
                Version = "v1",
                Description = "İçerik Yönetim Sistemi API'si - Clean Architecture implementasyonu",
                Contact = new Microsoft.OpenApi.Models.OpenApiContact
                {
                    Name = "NetCore CMS Team",
                    Email = "support@netcorecms.com"
                }
            });

            // XML comments for better documentation
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });

        // CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        return services;
    }

    public static IServiceCollection AddMapsterConfiguration(this IServiceCollection services)
    {
        // Mapster basit konfigürasyon - global settings 
        // Default konfigürasyon yeterli
        
        return services;
    }
} 