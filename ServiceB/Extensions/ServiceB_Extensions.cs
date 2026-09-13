using Confluent.Kafka;
using ServiceC;

namespace ServiceB.Extensions;

public static class ServiceB_Extensions
{
    public static IServiceCollection AddServiceB(this IServiceCollection services)
    {
        services.AddHostedService<Worker>();
        services.AddGrpcClient<Weather.WeatherClient>(options =>
            {
                options.Address = new Uri("http://localhost:5292");
            })
            .ConfigureChannel(channel =>
            {
                channel.HttpHandler = new SocketsHttpHandler
                {
                    EnableMultipleHttp2Connections = true,
                    PooledConnectionLifetime = TimeSpan.FromMinutes(10)
                };
            });

        return services;
    }

    public static ConsumerConfig GetConfig(this IConfiguration configuration)
    {
        var config = new ConsumerConfig 
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            GroupId = "service-b-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        return config;
    }
}