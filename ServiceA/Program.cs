using ServiceA.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddServiceA(builder.Configuration);

var host = builder.Build();

host.Run();
