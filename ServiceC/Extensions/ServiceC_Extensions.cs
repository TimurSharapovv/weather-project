using ServiceC.DataCleaning;
using ServiceC.DataBase;
using ServiceC.Storage;
using Microsoft.EntityFrameworkCore;
using ServiceC.Services;

namespace ServiceC.Extensions;

public static class ServiceC_Extensions
{
    public static IServiceCollection AddWeatherRestApi(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        return services;
    }
    
    public static IServiceCollection AddWeatherGrpcServer(this IServiceCollection services)
    {
        services.AddGrpc();
        return services;
    }
    
    public static IServiceCollection AddWeatherDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });
        return services;
    }
    
    public static IServiceCollection AddWeatherStorage(this IServiceCollection services)
    {
        services.AddScoped<IWeatherStorage, WeatherStorage>();
        return services;
    }
    
    public static IServiceCollection AddDataCleanup(this IServiceCollection services)
    {
        services.AddScoped<IDataCleanupService, DataCleanupService>();
        services.AddHostedService<DataCleaner>();
        return services;
    }
    
    public static ConfigureWebHostBuilder ConfigureWeatherKestrel(this ConfigureWebHostBuilder  kestrel)
    {
        kestrel.ConfigureKestrel(options =>
        {
            // Порт для REST API
            options.ListenLocalhost(5291, listenOptions => 
            { 
                listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1; 
            });

            // Порт для gRPC
            options.ListenLocalhost(5292, listenOptions =>
            {
                listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
            });
        });
        return kestrel;
    }
    
    public static WebApplication UseSwagger(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            SwaggerBuilderExtensions.UseSwagger(app);
            app.UseSwaggerUI();
        }
        return app;
    }


    public static WebApplication InitializeWeatherDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        db.Database.EnsureCreated(); 
        
        return app;
    }
}