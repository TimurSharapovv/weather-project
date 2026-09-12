using ServiceC.Storage;
using Grpc.Core;


namespace ServiceC.Services;

public class WeatherService(IWeatherStorage storage) : Weather.WeatherBase
{
    private readonly IWeatherStorage _storage = storage;
    public async override Task<Response> SetWeather(Request request, ServerCallContext context)
    {
        await _storage.SaveWeatherRecord(request, context.CancellationToken);
        return new Response { Success = true };
    }
}