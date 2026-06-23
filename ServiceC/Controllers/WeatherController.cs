using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;

namespace ServiceC.Controllers;

[ApiController]
[Route("api/Weather")]
public class WeatherController(ConcurrentQueue<Request> queue) : ControllerBase
{
    [HttpGet]
    public IEnumerable<Request> Get() 
    {
        //todo с точки зрения архитектуры ты долежен складывать записи лучше в постгресс, но даже если и хранишь их в очереди, 
        // то место обращения к ней должно быть не тут, а в классе типа interactor или другого паттерна (почитай) 
        return queue.Reverse().Take(10);
    }
}