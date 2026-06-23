using System.Collections.Concurrent;
using ServiceC;
using ServiceC.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddSingleton<ConcurrentQueue<Request>>();

builder.WebHost.ConfigureKestrel(options =>
{
    // Порт для REST API (HTTP/1.1)
    options.ListenLocalhost(5291, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1;
    });

    // Порт для gRPC (HTTPS + HTTP/2)
    options.ListenLocalhost(5292, listenOptions =>
    {
        listenOptions.UseHttps();
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<WeatherService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
app.MapControllers();

app.Run();
