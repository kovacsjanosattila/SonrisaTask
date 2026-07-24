namespace AIAgent.Contracts;

public interface IScheduledAgentTask
{
    // A logolás és a nyomon követhetőség miatt érdemes nevet adni
    string TaskName { get; } 
    
    // Maga a végrehajtó metódus
    Task<string> ExecuteAsync(CancellationToken cancellationToken);
}