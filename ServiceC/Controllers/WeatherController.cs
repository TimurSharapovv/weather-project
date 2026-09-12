using ServiceC.Storage;
using Microsoft.AspNetCore.Mvc;

namespace ServiceC.Controllers;

[ApiController]
[Route("api/Weather")]
public class WeatherController(IWeatherStorage storage) : ControllerBase
{
    private readonly IWeatherStorage _storage = storage;
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
    {
        var result = await _storage.GetLastRecordsAsync(10, cancellationToken);
        return Ok(result);
    }
}