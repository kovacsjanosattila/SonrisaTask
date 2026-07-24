using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace AIAgent.NewsService;

public class NewsPlugin
{
    private readonly INewsService _newsService;

    public NewsPlugin(INewsService NewsService)
    {
        _newsService = NewsService;
    }

    // CONSIDER: Maybe it will be much user focused to use general news_category (to eliminate unwanted celebrity-tabloid newses)
    // Then user can control news content to: politican, economic, technology, renewable ... directions
    [KernelFunction("get_news_data")]
    [Description("Megnézi egy adott ország politikai, gazdasági és egyéb híreit.")]
    public async Task<string> GetActualNews([Description("Az ország pontos neve, például Magyarország.")]string country)
    {
        return await _newsService.GetActualNewsAsync(country);
    }

}