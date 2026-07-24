namespace AIAgent.WeatherService;

public interface IWeatherService
{
    Task<string> GetCurrentWeatherAsync(string city, CancellationToken cancellationToken);
}
