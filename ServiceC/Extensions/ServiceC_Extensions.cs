using System.Collections.Concurrent;
using ServiceC.DataBase;
using Microsoft.EntityFrameworkCore;
using ServiceC.Storage;

namespace ServiceC.Extensions;

public static class ServiceC_Extensions
{
    public static IServiceCollection AddServiceC(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddControllers();
        
        services.AddGrpc();
        services.AddScoped<IWeatherStorage, WeatherStorageService>();
        
        return services;
        
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });
        return services;
    }

    public static ConfigureWebHostBuilder ConfigureKestrel(this ConfigureWebHostBuilder kestrel)
    {
        kestrel.ConfigureKestrel(options =>
        {
            // ���� ��� REST API (HTTP/1.1)
            options.ListenLocalhost(5291,
                listenOptions => { listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1; });

            // ���� ��� gRPC (HTTPS + HTTP/2)
            options.ListenLocalhost(5292, listenOptions =>
            {
                listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
            });
        });
        return kestrel;
    }

    public static WebApplication ImplementSwagger(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        return app;
    }
}