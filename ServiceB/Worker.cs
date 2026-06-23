using Confluent.Kafka;
using ServiceC;

namespace ServiceB
{
    public class Worker : BackgroundService
    {
        private readonly Weather.WeatherClient _grpcClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<Worker> _logger;

        public Worker(
            Weather.WeatherClient grpcClient,
            IConfiguration configuration,
            ILogger<Worker> logger)
        {
            _grpcClient = grpcClient;
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig 
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"],
                GroupId = "service-b-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            using var consumer = new ConsumerBuilder<string, byte[]>(config).Build();
            consumer.Subscribe(_configuration["Kafka:Topic"]);
            try 
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var message = consumer.Consume(stoppingToken);
                        if (message == null) continue;
                        var res = Request.Parser.ParseFrom(message.Message.Value);
                        await _grpcClient.SetWeatherAsync(res);
                        _logger.LogInformation("Получено сообщение из Kafka. Температура: {Temperature}, Влажность: {Humidity}, Описание: {Description}, Время: {Time}",
                            res.Temperature, res.Humidity, res.Description, res.Time);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Consume error occurred");
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
}
