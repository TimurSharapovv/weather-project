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

    public static ProducerConfig GetConfig(this IConfiguration cfg)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = cfg["Kafka:BootstrapServers"]
        };
        return config;
    }
    
    public static string GetEndpoit(IConfiguration cfg)
    {
        return $"{cfg["Open-Meteo:BaseUrl"]}?latitude={cfg["Open-Meteo:Latitude"]}" + $"&longitude={cfg["Open-Meteo:Longitude"]}" + $"&current={cfg["Open-Meteo:CurrentWeather"]}";
    }
    
    private static readonly Dictionary<int, string> Code = new()
    {
        { 0, "Clear sky" },
        { 1, "Mainly clear" },
        { 2, "Partly cloudy" },
        { 3, "Overcast" },
        { 45, "Fog" },
        { 48, "Depositing rime fog" },
        { 51, "Drizzle: Light intensity" },
        { 53, "Drizzle: Moderate intensity" },
        { 55, "Drizzle: Dense intensity" },
        { 56, "Freezing Drizzle: Light intensity" },
        { 57, "Freezing Drizzle: Dense intensity" },
        { 61, "Rain: Slight intensity" },
        { 63, "Rain: Moderate intensity" },
        { 65, "Rain: Heavy intensity" },
        { 66, "Freezing Rain: Light intensity" },
        { 67, "Freezing Rain: Heavy intensity" },
        { 71, "Snow fall: Slight intensity" },
        { 73, "Snow fall: Moderate intensity" },
        { 75, "Snow fall: Heavy intensity" },
        { 77, "Snow grains" },
        { 80, "Rain showers: Slight intensity" },
        { 81, "Rain showers: Moderate intensity" },
        { 82, "Rain showers: Violent intensity" },
        { 85, "Snow showers: Slight intensity" },
        { 86, "Snow showers: Heavy intensity" },
        { 95, "Thunderstorm: Slight or moderate" },
        { 96, "Thunderstorm with slight hail" },
        { 99, "Thunderstorm with heavy hail" }
    };

    public static string ConvertCode(int num)
    {
        Code.TryGetValue(num, out var result);
        return result ?? "";
    }
}