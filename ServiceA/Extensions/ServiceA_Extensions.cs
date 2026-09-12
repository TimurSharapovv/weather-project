using Confluent.Kafka;

namespace ServiceA.Extensions;

public static class ServiceA_Extensions
{
    public static IServiceCollection AddServiceA(this IServiceCollection services, IConfiguration configuration)
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
        services.AddHostedService<Worker>();
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
}