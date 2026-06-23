using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Grpc.Core;
using ServiceC;


namespace ServiceC.Services;

public class WeatherService : Weather.WeatherBase
{
    private readonly ConcurrentQueue<Request> _que;
    public WeatherService(ConcurrentQueue<Request> q)
    {
        _que = q;
    }
    public override Task<Response> SetWeather(Request request, ServerCallContext context)
    {
        _que.Enqueue(request);
        if (_que.Count > 10) 
        {
            _que.TryDequeue(out var _);
        }
        return Task.FromResult(new Response { Success = true });
    }
}