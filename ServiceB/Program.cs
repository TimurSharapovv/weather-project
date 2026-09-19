using ServiceB.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWeatherGrpcClient();
builder.Services.AddWeatherWorker();

var host = builder.Build();
host.Run();