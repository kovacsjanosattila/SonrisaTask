using AIAgentWorkerService;
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
