namespace WeatherService;

public interface IWeatherService
{
    Task<string> GetCurrentWeather(string city);
}
