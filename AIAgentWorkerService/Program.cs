using Microsoft.SemanticKernel;
using AIAgent.Contracts;
using AIAgent.WeatherService;
//using FinanceService;
//using RssNewsService;
using Serilog;

namespace AIAgentWorkerService;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.File(@"C:\Temp\Jani.txt")
            .CreateLogger();

        try
        {
            Log.Information("Starting up the service");
            
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            // Kernel builder beregisztrálása (Ebből tudunk új, tiszta Kerneleket gyártani)
            builder.Services.AddTransient<IKernelBuilder>(sp => 
                Kernel.CreateBuilder().AddOpenAIChatCompletion("gpt-4o", "API_KULCS"));

            // Pluginok
            builder.Services.AddTransient<WeatherPlugin>();
            //builder.Services.AddTransient<FinancePlugin>();
            //builder.Services.AddTransient<NewsPlugin>();

            // Az Agent Taskok regisztrálása UGYANAHHOZ az interfészhez
            builder.Services.AddScoped<IScheduledAgentTask>(sp => 
            {
                var builder = sp.GetRequiredService<IKernelBuilder>();
                return new WeatherAgentTask(builder.Build(), sp.GetRequiredService<WeatherPlugin>());
            });

            // builder.Services.AddScoped<IScheduledAgentTask>(sp => 
            // {
            //     var builder = sp.GetRequiredService<IKernelBuilder>();
            //     return new FinanceAgentTask(builder.Build(), sp.GetRequiredService<FinancePlugin>());
            // });

            // builder.Services.AddScoped<IScheduledAgentTask>(sp => 
            // {
            //     var builder = sp.GetRequiredService<IKernelBuilder>();
            //     return new NewsAgentTask(builder.Build(), sp.GetRequiredService<RssNewsPlugin>());
            // });

            
            // Register the Worker class as a hosted service to run background tasks.
            builder.Services
                .AddHostedService<Worker>()
                .AddSerilog();

            var host = builder.Build();
            host.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "There was a problem starting the service");
            return;
        }
        finally
        {
            Log.CloseAndFlush();
        }


    }
}
