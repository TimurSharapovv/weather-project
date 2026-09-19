using Confluent.Kafka;
using ServiceB.Extensions;
using ServiceC;

namespace ServiceB;

public class Worker(
    Weather.WeatherClient grpcClient,
    IConfiguration configuration,
    ILogger<Worker> logger)
    : BackgroundService
{
    private readonly ILogger<Worker> _logger = logger;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var consumer = new ConsumerBuilder<string, byte[]>(ServiceB_Extensions.GetConfig(configuration)).Build();
        consumer.Subscribe(configuration["Kafka:Topic"]);
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var message = consumer.Consume(stoppingToken);
                    var response = Request.Parser.ParseFrom(message.Message.Value);
                    await grpcClient.SetWeatherAsync(response, cancellationToken: stoppingToken);
                    _logger.LogInformation(
                        "Got new data from Kafka. Temperature: {Temperature}, Humidity: {Humidity}, Description: {Description}, Time: {Time}",
                        response.Temperature, response.Humidity, response.Description, response.Time);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Consume error occurred");
                    await Task.Delay(60000, stoppingToken);
                }
            }
        }
        finally 
        {
            consumer.Close();
        }
    }
}