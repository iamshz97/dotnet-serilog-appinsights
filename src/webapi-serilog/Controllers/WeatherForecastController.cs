using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace webapi_serilog.Controllers;
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
        var weatherForecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();

        var orderNumber = 1;

        _logger.LogInformation(
            "Processing weatherForecast {@weatherForecast}, order number = {@OrderNumber}",
            weatherForecasts,
            orderNumber);

        _logger.LogInformation("Weather {Count}", 9);
        _logger.LogInformation("Weather {Count}", 10);
        _logger.LogInformation("Weather {Count}", 11);

        return weatherForecasts;
    }
}
