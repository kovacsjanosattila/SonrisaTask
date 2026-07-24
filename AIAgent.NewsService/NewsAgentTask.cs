
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
        chatHistory.AddUserMessage("Ha a helyi politikát vagy gazdaságot a megszokottól jobban-mélyebben érintő eseményt látsz küldj riasztást !");
        
        var agentResponse = new System.Text.StringBuilder();
        await foreach (var message in _agent.InvokeAsync(chatHistory, cancellationToken: cancellationToken)) 
        {
            agentResponse.Append(message.Message);
        }
        return agentResponse.ToString();
    }
}
