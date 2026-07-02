using System.Collections.Concurrent;
using Grpc.Core;


namespace ServiceC.Services;

public class WeatherService(ConcurrentQueue<Request> queue) : Weather.WeatherBase
{
    public override Task<Response> SetWeather(Request request, ServerCallContext context)
    {
        lock (queue) //fixed: блокируем доступ к очереди для обеспечения потокобезопасности
        {
            queue.Enqueue(request);
            if (queue.Count > 10)
                queue.TryDequeue(out _);
            return Task.FromResult(new Response { Success = true });
        }
    }
}