using ServiceB;
using ServiceC;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddGrpcClient<Weather.WeatherClient>(options =>
{
    options.Address = new Uri("https://localhost:5292");
})
.ConfigureChannel(channel =>
{
    channel.HttpHandler = new SocketsHttpHandler
    {
        EnableMultipleHttp2Connections = true,
        PooledConnectionLifetime = TimeSpan.FromMinutes(10)
    };
});

var host = builder.Build();
host.Run();

