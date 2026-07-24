namespace AIAgent.NewsService;

public interface INewsService
{
    Task<string> GetActualNewsAsync(string country, CancellationToken cancellationToken = default);
}
