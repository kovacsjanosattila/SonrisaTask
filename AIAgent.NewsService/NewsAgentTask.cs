
using AIAgent.Contracts;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AIAgent.NewsService;

public class NewsAgentTask : IScheduledAgentTask
{
    private readonly ChatCompletionAgent _agent;
    public string TaskName => "NewsAggregator";

    public NewsAgentTask(Kernel kernel, NewsPlugin newsPlugin)
    {
        kernel.Plugins.AddFromObject(newsPlugin);
        _agent = new ChatCompletionAgent
        {
            Name = "NewsAgent",
            Instructions = "Olvasd be a megadott hírportálok RSS feedjét, és készíts egy 5 mondatos magyar nyelvű összefoglalót az egyes hírekről.",
            Kernel = kernel
        };
    }

    public async Task<string> ExecuteAsync(CancellationToken cancellationToken)
    {
        var chatHistory = new ChatHistory();
        chatHistory.AddSystemMessage("Te egy szigorú adatellenőrző asszisztens vagy. "
            + " Csak a megadott országhoz kapcsolódó politikai vagy gazdasági híreket ved fel a válaszodba. Derítsd ki a hír megjelenési dátumát is. Ha az egy hétnél régebbi hagyd ki a hírt!" 
            + " Ha nincs megemlítendő hír akkor mond azt hogy 'Nincs semmi különös hír'!");
        chatHistory.AddUserMessage("Ha a helyi politikát vagy gazdaságot a megszokottól erősebben vagy mélyebben érintő eseményt látsz küldj riasztást !");
        
        var agentResponse = new System.Text.StringBuilder();
        await foreach (var message in _agent.InvokeAsync(chatHistory, cancellationToken: cancellationToken)) 
        {
            agentResponse.Append(message.Message);
        }
        return agentResponse.ToString();
    }
}
