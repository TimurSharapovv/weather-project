using System.Text.Json;
using Confluent.Kafka;
using Google.Protobuf;
using ServiceA.Models;
using ServiceA.Extensions;
using ServiceC;

namespace ServiceA;

public class Worker(
    ILogger<Worker> logger,
    IHttpClientFactory httpclient,
    IConfiguration configuration)
    : BackgroundService
{
    private readonly ILogger<Worker> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var producer = new ProducerBuilder<string, byte[]>(ServiceA_Extensions.GetConfig(configuration)).Build();
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var req = httpclient.CreateClient("OpenMeteo");
                var url = ServiceA_Extensions.GetEndpoit(configuration);

                _logger.LogInformation("Requesting URL: {Url}", url);

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
                    Description = ServiceA_Extensions.ConvertCode(response.Current.WeatherCode),
                    Time =  DateTime.Now.ToString("g")
                };
                var weatherMessage = new Request 
                {
                    Temperature = weatherdto.Temperature,
                    Humidity = Convert.ToInt32(weatherdto.Humidity),
                    Description = weatherdto.Description,
                    Time = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(
                        DateTime.SpecifyKind(Convert.ToDateTime(weatherdto.Time), DateTimeKind.Utc))
                };
                await producer.ProduceAsync(configuration["Kafka:Topic"],
                    new Message<string, byte[]> { Key = "Kazan", Value = weatherMessage.ToByteArray() }, stoppingToken);
                _logger.LogInformation("����� ������ - {Temperature}, {Humidity}, {Description}, {Time}",
                    weatherMessage.Temperature, weatherMessage.Humidity, weatherMessage.Description,
                    weatherMessage.Time);
                await Task.Delay(60000, stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Consume error occurred");
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}