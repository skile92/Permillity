using PermillityTest.Persistence;

namespace PermillityTest.Services;

public class WeatherService : IWeatherService
{
    private readonly MyDbContext _context;

    public WeatherService(MyDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(WeatherForecast entity)
    {
        _context.WeatherForecasts.Add(entity);

        await _context.SaveChangesAsync();
    }

    public Task<List<WeatherForecast>> GetAsync()
    {
        var data = _context.WeatherForecasts.ToList();

        return Task.FromResult(data);
    }

    public Task<WeatherForecast> GetAsync(int id)
    {
        var data = _context.WeatherForecasts.FirstOrDefault(x => x.Id == id);

        return Task.FromResult(data);
    }
}
