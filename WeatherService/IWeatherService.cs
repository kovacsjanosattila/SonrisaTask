namespace WeatherService;

public interface IWeatherService
{
    Task<string> GetCurrentWeatherAsync(string city);
}
