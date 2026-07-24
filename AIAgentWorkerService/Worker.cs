using AIAgent.Contracts;
using AIAgent.WeatherService;
namespace AIAgentWorkerService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;


    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _logger.LogInformation("Worker service instance (singleton) created at: {time}", DateTimeOffset.Now);
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Worker service starting at: {time}", DateTimeOffset.Now);   
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Worker service stopping at: {time}", DateTimeOffset.Now);
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
            // Each cicle has a new scope. It helps garbage collector to release-collect resources
            using (var scope = _scopeFactory.CreateScope())
            {
                // Lekéri az ÖSSZES regisztrált Agent implementációt
                var agentTasks = scope.ServiceProvider.GetRequiredService<IEnumerable<IScheduledAgentTask>>();
                
                _logger.LogInformation("Elindul {Count} db Agent task feldolgozása...", agentTasks.Count());

                // Párhuzamos futtatás előkészítése
                var executingTasks = new List<Task>();

                foreach (var agentTask in agentTasks)
                {
                    _logger.LogInformation("Agent indítása: {TaskName}", agentTask.TaskName);
                    
                    // Indítjuk a feladatot, de nem tesszük elé az 'await' kulcsszót a cikluson belül
                    var task = agentTask.ExecuteAsync(stoppingToken) // Ez most már Task<string>
                        .ContinueWith(t => 
                        {
                            // Feladatszintű hibakezelés, hogy ha az egyik elszáll, a többi lefusson
                            if (t.IsFaulted)
                            {
                                _logger.LogError(t.Exception, "Hiba történt a(z) {TaskName} futtatása közben.", agentTask.TaskName);
                            }
                            else if (!t.IsCanceled)
                            {
                                // Itt már hozzáférünk az Agent válaszához a t.Result-on keresztül
                                _logger.LogInformation("Agent ({TaskName}) sikeresen lefutott. Válasz: {AgentResponse}", agentTask.TaskName, t.Result);
                            }
                        }, stoppingToken);

                    executingTasks.Add(task);
                }

                // Megvárjuk, amíg az összes Agent befejezi a munkát (vagy kivételt dob)
                await Task.WhenAll(executingTasks);
                
                _logger.LogInformation("Minden Agent végzett. Várakozás a következő ciklusra.");
            }

            // Wait 7 seconds delay
            await Task.Delay(7 * 1000, stoppingToken);
        }
    }
}
