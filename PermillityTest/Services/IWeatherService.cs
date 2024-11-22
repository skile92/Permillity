using PermillityTest.Persistence;

namespace PermillityTest.Services;

public interface IWeatherService
{
    Task<List<WeatherForecast>> GetAsync();
    Task<WeatherForecast> GetAsync(int id);
    Task AddAsync(WeatherForecast entity);
}
