using NetCoreCase.API.Extensions;
using NetCoreCase.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddApiServices();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddMapsterConfiguration();

// Add logging
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "NetCore CMS API v1");
        c.RoutePrefix = string.Empty; // Swagger UI'ı root'ta açar
        c.DocumentTitle = "NetCore CMS API";
        c.DefaultModelsExpandDepth(-1); // Model'leri kapalı gösterir
    });
}

// Security headers
app.UseHsts();
app.UseHttpsRedirection();

// CORS
app.UseCors("AllowAll");

// Custom middleware
app.UseMiddleware<ErrorHandlingMiddleware>();

// Authentication & Authorization (şimdilik yok)
// app.UseAuthentication();
// app.UseAuthorization();

// Controllers
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => new
{
    Status = "Healthy",
    Timestamp = DateTime.UtcNow,
    Version = "1.0.0",
    Environment = app.Environment.EnvironmentName
});

// API bilgi endpoint'i
app.MapGet("/api/info", () => new
{
    Title = "NetCore CMS API",
    Version = "1.0.0",
    Description = "İçerik Yönetim Sistemi API'si - Clean Architecture implementasyonu",
    Documentation = "/swagger",
    Endpoints = new
    {
        Users = "/api/users",
        Categories = "/api/categories",
        Contents = "/api/contents",
        Health = "/health"
    }
});

app.Run();
