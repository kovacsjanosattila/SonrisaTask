using Microsoft.SemanticKernel;
using AIAgent.Contracts;
using AIAgent.WeatherService;
//using FinanceService;
//using RssNewsService;
using Serilog;
using AIAgent.NewsService;

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

            // Plugins
            builder.Services.AddTransient<WeatherPlugin>();
            builder.Services.AddTransient<NewsPlugin>();
            //builder.Services.AddTransient<FinancePlugin>();

            // Services for Plugins
            builder.Services.AddHttpClient<IWeatherService, OpenWeatherService>();
            builder.Services.AddHttpClient<INewsService, RssNewsService>();

            // Agent Tasks which relay on same IScheduledAgentTask interface
            builder.Services.AddScoped<IScheduledAgentTask>(sp => 
            {
                var builder = sp.GetRequiredService<IKernelBuilder>();
                return new WeatherAgentTask(builder.Build(), sp.GetRequiredService<WeatherPlugin>());
            });
            builder.Services.AddScoped<IScheduledAgentTask>(sp => 
            {
                var builder = sp.GetRequiredService<IKernelBuilder>();
                return new NewsAgentTask(builder.Build(), sp.GetRequiredService<NewsPlugin>());
            });
            // builder.Services.AddScoped<IScheduledAgentTask>(sp => 
            // {
            //     var builder = sp.GetRequiredService<IKernelBuilder>();
            //     return new FinanceAgentTask(builder.Build(), sp.GetRequiredService<FinancePlugin>());
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
