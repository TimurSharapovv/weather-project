using Confluent.Kafka;
using ServiceA;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHttpClient("OpenMeteo", client =>
{
    client.DefaultRequestHeaders.UserAgent.ParseAdd("WeatherService/1.0");
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    SslProtocols = System.Security.Authentication.SslProtocols.Tls12 |
                   System.Security.Authentication.SslProtocols.Tls13
});
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<IProducer<string, byte[]>>(sp =>
{
    var config = new ProducerConfig
    {
        BootstrapServers = builder.Configuration["Kafka:BootstrapServers"]
    };
    return new ProducerBuilder<string, byte[]>(config).Build();
});

var host = builder.Build();

host.Run();
