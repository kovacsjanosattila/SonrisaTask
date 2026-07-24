using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace AIAgent.WeatherService;

public class OpenWeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenWeatherService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenWeather:ApiKey"];
    }

    public async Task<string> GetCurrentWeatherAsync(string city,CancellationToken cancellationToken = default)
    {
        var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}";
        
        var response = await _httpClient.GetStringAsync(url,cancellationToken);
        
        var json = JsonDocument.Parse(response);

        var temp = json.RootElement
            .GetProperty("main")
            .GetProperty("temp")
            .GetDouble();

        var description = json.RootElement
            .GetProperty("weather")[0]
            .GetProperty("description")
            .GetString();

        return $"{city}: {temp}°C, {description}";
    }
}