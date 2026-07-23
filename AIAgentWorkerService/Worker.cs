namespace AIAgentWorkerService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private HttpClient client;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
        _logger.LogInformation("Worker service instance (singleton) created at: {time}", DateTimeOffset.Now);
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Worker service starting at: {time}", DateTimeOffset.Now);   
        client = new HttpClient();
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Worker service stopping at: {time}", DateTimeOffset.Now);
        client.Dispose();
        return base.StopAsync(cancellationToken);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns> 
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        while (!stoppingToken.IsCancellationRequested)
        {
            // It is just a testing content, I will change it later ....
            var result = await client.GetAsync("http://fonixvet.hu");
            if(result.IsSuccessStatusCode)
            {
                _logger.LogInformation("Fonixvet elérhető. Státusz kód: {StatusCode}", result.StatusCode);
            }
            else
            {
                _logger.LogError("Fonixvet nem elérhető. Státusz kód: {StatusCode}", result.StatusCode);
            }
            // Wait 7 seconds delay
            await Task.Delay(7 * 1000, stoppingToken);
        }
    }
}
