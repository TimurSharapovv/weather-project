using ServiceC.Extensions;
using ServiceC.Services;

var builder = WebApplication.CreateBuilder();


builder.WebHost.ConfigureWeatherKestrel();

builder.Services
    .AddWeatherRestApi()
    .AddWeatherGrpcServer()
    .AddWeatherDatabase(builder.Configuration)
    .AddWeatherStorage()
    .AddDataCleanup();



var app = builder.Build();

app.InitializeWeatherDatabase()
    .UseSwagger();

app.MapControllers();
app.MapGrpcService<WeatherService>();

app.Run();