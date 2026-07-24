using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace AIAgent.WeatherService;

public class WeatherPlugin
{
    private readonly IWeatherService _weatherService;

    public WeatherPlugin(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [KernelFunction("get_weather_data")]
    [Description("Lekérdezi hogy milyen idő várható a megadott településen. Megnézi hogy várható-e vihar vagy extrém időjárás.")]
    public async Task<string> GetCurrentWeather([Description("A település pontos neve, például Budapest")]string city,CancellationToken cancellationToken = default)
    {
        return await _weatherService.GetCurrentWeatherAsync(city,cancellationToken);
    }

}