using ServiceB.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddServiceB();

var host = builder.Build();
host.Run();