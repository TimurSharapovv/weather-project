using ServiceC.Extensions;
using ServiceC.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceC();

builder.WebHost.ConfigureKestrel();

builder.Services.AddDatabase(builder.Configuration);

var app = builder.Build();

app.ImplementSwagger();

app.MapGrpcService<WeatherService>();
app.MapControllers();

app.Run();