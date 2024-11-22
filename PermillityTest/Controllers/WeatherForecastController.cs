using Microsoft.AspNetCore.Mvc;
using PermillityTest.Persistence;
using PermillityTest.Services;

namespace PermillityTest.Controllers;

[ApiController]
public class WeatherForecastController : ControllerBase
{    
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IWeatherService _weatherService;

    public WeatherForecastController(IWeatherService weatherService, ILogger<WeatherForecastController> logger)
    {
        _weatherService = weatherService;
        _logger = logger;
    }

    [HttpGet]
    [Route("api/GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        _logger.LogInformation("Test log");

        return await _weatherService.GetAsync();
    }

    [HttpGet]
    [Route("api/GetWeatherForecast/{id}")]
    public async Task<WeatherForecast> Get(int id)
    {
        return await _weatherService.GetAsync(id);
    }

    [HttpPost]
    [Route("api/GetWeatherForecast")]
    public async Task<ActionResult> Create([FromBody] CreateWeatherForecast createWeatherForecast)
    {
        var entity = new WeatherForecast()
        {
            Summary = createWeatherForecast.Summary,
            Date = createWeatherForecast.Date,
            TemperatureC = 10
        };

        await _weatherService.AddAsync(entity);

        return Ok();
    }

    [HttpDelete]
    [Route("api/GetWeatherForecast/{test}")]
    public async Task<ActionResult> Delete(int test)
    {
        return Ok();
    }
}
