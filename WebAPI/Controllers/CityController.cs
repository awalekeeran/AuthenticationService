using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Data;

namespace WebAPI.Controllers
{
    [Authorize]
    public class CityController : BaseController
    {
        private static readonly string[] Cities = new[]
        {
            "Atlanta", "New York", "Pune", "Bengaluru"
        };

        private readonly DataContext dataContext;

        private readonly ILogger<CityController> logger;

        public CityController(ILogger<CityController> logger, DataContext dataContext)
        {
            this.logger = logger;
            this.dataContext = dataContext;
        }

        [HttpGet(Name = "GetAllCity")]
        public IActionResult Get()
        {
            var cities = dataContext.Cities.ToList() ;

            return Ok(cities);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return Cities[id];
        }
    }
}
