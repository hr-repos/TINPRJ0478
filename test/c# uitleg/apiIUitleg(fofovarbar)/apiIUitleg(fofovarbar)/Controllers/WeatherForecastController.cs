using apiIUitleg_fofovarbar_.Controllers.testBody;
using Microsoft.AspNetCore.Mvc;

namespace apiIUitleg_fofovarbar_.Controllers
{
    [ApiController]
    [Route("/api")]
    public class WeatherForecastController : ControllerBase
    {

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpPost("test")]
        public ActionResult Get(respuistTestBody requist)
        {
            var bar = new responseTestBody
            { 
                foo = 200
            };

            bar.foo = 200;

            if (requist == null)
                return BadRequest();

            return Ok
            (
                new responseTestBody 
                { 
                    foo = 200 
                } 
            );
        }
    }
}