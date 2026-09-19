using ServiceA.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOpenMeteoClient(builder.Configuration);
builder.Services.AddKafkaProducer(builder.Configuration);
builder.Services.AddWeatherWorker();

var host = builder.Build();

host.Run();
