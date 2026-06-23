using System;
using System.Net.Http;
using System.Text.Json;
using Confluent.Kafka;
using Google.Protobuf;
using ServiceA.Models;
using System.Security.Authentication;
using ServiceC;

namespace ServiceA
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHttpClientFactory _httpclient;
        private readonly IConfiguration _configuration;
        private static readonly Dictionary<int, string> code = new()
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
        public string ConvertCode(int num) 
        {
            code.TryGetValue(num, out string? result);
            return result ?? "";
        }

        public Worker(ILogger<Worker> logger, IHttpClientFactory httpclient, IConfiguration cfg)
        {
            _logger = logger;
            _httpclient = httpclient;
            _configuration = cfg;
        }
        

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"]
            };
            using var producer = new ProducerBuilder<string, byte[]>(config).Build();
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {

                    var req = _httpclient.CreateClient("OpenMeteo");
                    var url = $"{_configuration["Open-Meteo:BaseUrl"]}?latitude={_configuration["Open-Meteo:Latitude"]}&longitude={_configuration["Open-Meteo:Longitude"]}&current={_configuration["Open-Meteo:CurrentWeather"]}";
                    _logger.LogInformation("Requesting URL: {Url}", url);
                    var wthr = await req.GetAsync(url, stoppingToken);
                    if (wthr.IsSuccessStatusCode == false)
                    {
                        throw new Exception("Запрос к API неуспешен!");
                    }
                    var message = await wthr.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var q = JsonSerializer.Deserialize<OpenMeteoResponse>(message, options);
                    if (q is null || q.Current is null || q.Current.Time is null)
                    {
                        throw new Exception("Не хватает данных о погоде!");
                    }
                    var request = new Request();
                    request.Temperature = (float)q.Current.Temperature2m;
                    request.Humidity = q.Current.RelativeHumidity2m;
                    request.Description = ConvertCode(q.Current.WeatherCode);
                    request.Time = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(
                        DateTime.SpecifyKind(DateTime.Parse(q.Current.Time), DateTimeKind.Utc)
                    );
                    await producer.ProduceAsync(_configuration["Kafka:Topic"], new Message<string, byte[]> { Key = "Kazan", Value = request.ToByteArray() }, stoppingToken);
                    _logger.LogInformation("Новая запись - {Temperature}, {Humidity}, {Description}, {Time}",
                        request.Temperature, request.Humidity, request.Description, request.Time);
                    await Task.Delay(10000, stoppingToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Consume error occurred");
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }
    }
}
