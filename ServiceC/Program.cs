using System.Collections.Concurrent;
using ServiceC;
using ServiceC.Services;

var builder = WebApplication.CreateBuilder(args);
    
builder.Services.AddEndpointsApiExplorer();//навалили свагги
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddSingleton<ConcurrentQueue<Request>>();

builder.WebHost.ConfigureKestrel(options =>
{
    // ���� ��� REST API (HTTP/1.1)
    options.ListenLocalhost(5291,
        listenOptions => { listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1; });

    // ���� ��� gRPC (HTTPS + HTTP/2)
    options.ListenLocalhost(5292, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())//еще немножечько свагги
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
app.MapGrpcService<WeatherService>();
app.MapControllers();

app.Run();