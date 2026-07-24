using Microsoft.SemanticKernel;
using AIAgent.Contracts;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AIAgent.WeatherService;
public class WeatherAgentTask : IScheduledAgentTask
{
    private readonly ChatCompletionAgent _agent;
    public string TaskName => "WeatherMonitor";

    public WeatherAgentTask(Kernel kernel, WeatherPlugin weatherPlugin)
    {
        // A Kernelbe csak az időj plugint injektáljuk
        kernel.Plugins.AddFromObject(weatherPlugin);

        _agent = new ChatCompletionAgent
        {
            Name = "WeatherAgent",
            Instructions = "Vizsgáld meg az aktuális 24 órás időjárás előrejelzést !",
            Kernel = kernel
        };
    }

    public async Task<string> ExecuteAsync(CancellationToken cancellationToken)
    {
        var chatHistory = new ChatHistory();
        chatHistory.AddUserMessage("Ha hirtelen változást, vagy szélsőséges időjárás előrejelzést látsz küldj riasztást !");
        
        var agentResponse = new System.Text.StringBuilder();
        await foreach (var message in _agent.InvokeAsync(chatHistory, cancellationToken: cancellationToken)) 
        {
            agentResponse.Append(message.Message);
        }
        return agentResponse.ToString();
    }
}
