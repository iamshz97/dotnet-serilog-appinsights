using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Events;
using webapi_serilog.Services;
using webapi_serilog.Services.Interface;

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
    private readonly ILoggingLevelSwitchService _loggingLevelSwitchService;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, ILoggingLevelSwitchService loggingLevelSwitchService)
    {
        _logger = logger;
        _loggingLevelSwitchService = loggingLevelSwitchService;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    [ProducesResponseType(typeof(IEnumerable<WeatherForecast>), StatusCodes.Status200OK)]
    public IActionResult Get(bool isFailRequest = false)
    {
        try
        {
            _logger.LogDebug("Processing weather forecast request");
            _logger.LogDebug("Accessing Database");

            var weatherForecasts = GetWeatherForecast();

            var avgTemperature = 1;

            _logger.LogInformation(
                "Processing weatherForecast {@weatherForecast}, avgTemperature = {avgTemperature}",
                weatherForecasts,
                avgTemperature);

            if (isFailRequest)
            {
                throw new Exception("Fail request");
            }

            return Ok(weatherForecasts);
        }
        catch
        {
            _loggingLevelSwitchService.SetLogLevel(LogEventLevel.Debug);
        }

        return BadRequest();
    }

    private IEnumerable<WeatherForecast> GetWeatherForecast()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        }).ToArray();
    }
}
