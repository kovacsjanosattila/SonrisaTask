using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AIAgent.WeatherService;

public class OpenWeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenWeatherService> _logger;
    private readonly string _apiKey;

    public OpenWeatherService(HttpClient httpClient, IConfiguration configuration, ILogger<OpenWeatherService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = configuration["OpenWeather:ApiKey"];
    }

    public async Task<string> GetCurrentWeatherAsync(string city,CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("GetCurrentWeatherAsync started!");

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

        _logger.LogInformation("GetCurrentWeatherAsync ending!");
        return $"{city}: {temp}°C, {description}";
    }
}