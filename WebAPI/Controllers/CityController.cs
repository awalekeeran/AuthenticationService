using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private static readonly string[] Cities = new[]
        {
            "Atlanta", "New York", "Pune", "Bengaluru"
        };

        private readonly ILogger<CityController> _logger;

        public CityController(ILogger<CityController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetCity")]
        public IEnumerable<string> Get()
        {
            return Cities;
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return Cities[id];
        }
    }
}
