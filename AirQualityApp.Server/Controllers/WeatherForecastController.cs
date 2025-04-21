using Microsoft.AspNetCore.Mvc;

namespace AirQualityApp.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("test")]
        public string Test(DateTime dateTime)
        {
            return dateTime.Year < 2025 ? "Hello, world!" : "No world!";
        }

        [HttpPost("test1")]
        public List<string> Test1(List<DateTime> dateTimes)
        {
            var output = new List<string>();
            foreach(var dateTime in dateTimes)
            {
                output.Add(dateTime.Year < 2025 ? "Hello, world!" : "No world!");
            }
            return output;
        }
    }
}
