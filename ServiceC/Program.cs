using ServiceC.Extensions;
using ServiceC.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceC(builder.Configuration);

builder.WebHost.ConfigureKestrel();


var app = builder.Build();

app.ImplementSwagger();

app.AddDataBase();

app.MapGrpcService<WeatherService>();
app.MapControllers();

app.Run();