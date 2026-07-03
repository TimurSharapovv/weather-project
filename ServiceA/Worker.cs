using System.Text.Json;
using Confluent.Kafka;
using Google.Protobuf;
using ServiceA.Models;
using ServiceC;

namespace ServiceA;

public class Worker(
    ILogger<Worker> logger,
    IHttpClientFactory httpclient,
    IConfiguration cfg)
    : BackgroundService
{
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

    private static string ConvertCode(int num)
    {
        Code.TryGetValue(num, out var result);
        return result ?? "";
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = cfg["Kafka:BootstrapServers"]
        };
        using var producer = new ProducerBuilder<string, byte[]>(config).Build();
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var req = httpclient.CreateClient("OpenMeteo");
                var url =
                    $"{cfg["Open-Meteo:BaseUrl"]}?latitude={cfg["Open-Meteo:Latitude"]}" +
                    $"&longitude={cfg["Open-Meteo:Longitude"]}" +
                    $"&current={cfg["Open-Meteo:CurrentWeather"]}";

                logger.LogInformation("Requesting URL: {Url}", url);

                var weather = await req.GetAsync(url, stoppingToken);
                if (weather.IsSuccessStatusCode == false)
                    throw new Exception("������ � API ���������!");

                var weatherApiResponse = await weather.Content.ReadAsStringAsync(stoppingToken);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var response = JsonSerializer.Deserialize<OpenMeteoResponse>(weatherApiResponse, options);
                if (response is null || response.Current is null || response.Current.Time is null)
                    throw new Exception("�� ������� ������ � ������!");
                var weatherdto = new WeatherDto
                {
                    Temperature = (float)response.Current.Temperature2M,
                    Humidity = response.Current.RelativeHumidity2M,
                    Description = ConvertCode(response.Current.WeatherCode),
                    Time = Convert.ToDateTime(response.Current.Time)
                };
                var weatherMessage = new Request 
                {
                    Temperature = weatherdto.Temperature,
                    Humidity = Convert.ToInt32(weatherdto.Humidity),
                    Description = weatherdto.Description,
                    Time = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(
                        DateTime.SpecifyKind(weatherdto.Time, DateTimeKind.Utc))
                };
                await producer.ProduceAsync(cfg["Kafka:Topic"],
                    new Message<string, byte[]> { Key = "Kazan", Value = weatherMessage.ToByteArray() }, stoppingToken);
                logger.LogInformation("����� ������ - {Temperature}, {Humidity}, {Description}, {Time}",
                    weatherMessage.Temperature, weatherMessage.Humidity, weatherMessage.Description,
                    weatherMessage.Time);
                await Task.Delay(10000, stoppingToken);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Consume error occurred");
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}