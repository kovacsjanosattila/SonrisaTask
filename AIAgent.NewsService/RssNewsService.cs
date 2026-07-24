using System.ServiceModel.Syndication;
using System.Text;
using System.Text.Json;
using System.Xml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AIAgent.NewsService;

public class RssNewsService : INewsService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RssNewsService> _logger;
    private readonly string _rssFeedUrl;
    private int _maxItems = 5;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpClient"></param>


    public RssNewsService(HttpClient httpClient, IConfiguration configuration, ILogger<RssNewsService> logger)
    {
        _httpClient = httpClient;
        _rssFeedUrl = configuration["RssNewsService:RssFeedUrl"];
        _maxItems = int.TryParse(configuration["RssNewsService:MaxItems"], out var intValue) ? intValue : 5;
        _logger = logger;

    }

    public async Task<string> GetActualNewsAsync(string country, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("GetActualNewsAsync started!");
        string returnValue = string.Empty;
        try
        {
            // 1. Download an RSS XML into one stream
            using var responseStream = await _httpClient.GetStreamAsync(_rssFeedUrl, cancellationToken);
            
            // 2. Create a safe XmlReadert 
            using var xmlReader = XmlReader.Create(responseStream, new XmlReaderSettings 
            { 
                DtdProcessing = DtdProcessing.Ignore, // Biztonsági okokból érdemes kikapcsolni
                Async = true 
            });

            // 3. Use a built in .NET class to read and parse the XML
            var feed = SyndicationFeed.Load(xmlReader);

            if (feed == null)
            {
                _logger.LogWarning("A letöltött RSS feed üres vagy érvénytelen: {Url}", _rssFeedUrl);
                return string.Empty;
            }

            // 4. Extract relevant data and limitate return data amount
            var articles = feed.Items
                .OrderByDescending(item => item.PublishDate)
                .Take(_maxItems)
                .Select(item => 
                    $"Cím: {item.Title?.Text ?? "Nincs cím"} Összegzett szöveg: {StripHtmlTags(item.Summary?.Text ?? string.Empty)} Megjelenése: {item.PublishDate}"
                )
                .ToList();

            // ToDo: News service may have more item (news) to return, but but Weather service only one
            returnValue = string.Join("\n", articles);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Hálózati hiba az RSS feed letöltésekor: {Url}", _rssFeedUrl);
            returnValue = $"Hálózati hiba az RSS feed letöltésekor: {_rssFeedUrl}";
        }
        catch (XmlException ex)
        {
            _logger.LogError(ex, "Hibás XML formátum a feedben: {Url}", _rssFeedUrl);
            returnValue = $"Az RSS feed formátuma hibás: {_rssFeedUrl}";
        }
        _logger.LogInformation("GetActualNewsAsync ending!");
        return returnValue;
    }

    // Segédmetódus, mert az RSS feedek summary-je gyakran tartalmaz nyers HTML elemeket (<p>, <br>), 
    // amit az AI modellnek felesleges feldolgoznia. (Ezt a metódust igény szerint fejlesztheted)
    private static string StripHtmlTags(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;
        return System.Text.RegularExpressions.Regex.Replace(input, "<.*?>", string.Empty).Trim();
    }
}