using Confluent.Kafka;

namespace ServiceA.Extensions;

public static class ServiceA_Extensions
{
    public static IServiceCollection AddOpenMeteoClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient("OpenMeteo", client =>
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("WeatherService/1.0");
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                SslProtocols = System.Security.Authentication.SslProtocols.Tls12 |
                               System.Security.Authentication.SslProtocols.Tls13
            });

        return services;
    }
    
    public static IServiceCollection AddKafkaProducer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IProducer<string, byte[]>>(sp =>
        {
            var config = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"]
            };
            return new ProducerBuilder<string, byte[]>(config).Build();
        });
        
        return services;
    }
    
    public static IServiceCollection AddWeatherWorker(this IServiceCollection services)
    {
        services.AddHostedService<Worker>();
        return services;
    }

    public static ProducerConfig GetConfig(this IConfiguration cfg)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = cfg["Kafka:BootstrapServers"]
        };
        return config;
    }
    
}