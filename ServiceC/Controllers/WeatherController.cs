using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;

namespace ServiceC.Controllers
{
    [ApiController]
    [Route("api/Weather")]
    public class WeatherController : ControllerBase
    {
        private readonly ConcurrentQueue<Request> _que;
        public WeatherController(ConcurrentQueue<Request> q)
        {
            _que = q;
        }
        [HttpGet]
        public IEnumerable<Request> Get() 
        {
            return _que.Reverse().Take(10);
        }
    }
}
