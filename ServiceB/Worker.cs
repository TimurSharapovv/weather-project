using Confluent.Kafka;
using ServiceC;

namespace ServiceB;

public class Worker(
    Weather.WeatherClient grpcClient,
    IConfiguration configuration,
    ILogger<Worker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig //todo вынести в другое место
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            GroupId = "service-b-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        using var consumer = new ConsumerBuilder<string, byte[]>(config).Build();
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
                    logger.LogInformation(
                        "�������� ��������� �� Kafka. �����������: {Temperature}, ���������: {Humidity}, ��������: {Description}, �����: {Time}",
                        response.Temperature, response.Humidity, response.Description, response.Time);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Consume error occurred");
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }
        finally 
        {
            consumer.Close();
        }
    }
}