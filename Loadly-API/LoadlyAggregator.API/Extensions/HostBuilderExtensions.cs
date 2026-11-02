using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using LoadlyAggregator.Infrastructure.Services.Logging;
namespace LoadlyAggregator.API.Extensions;



public static class HostBuilderExtensions
{




    public static IHostBuilder UseSerilogWithExporters(this IHostBuilder hostBuilder, IConfiguration configuration, IHostEnvironment environment)
    {

        var enabledExporters = LogExporterFactory.GetEnabledExporters(configuration);

        hostBuilder.UseSerilog((context, services, loggerConfiguration) =>
        {

            var defaultLevel = configuration["Serilog:MinimumLevel:Default"] ?? "Information";
            loggerConfiguration.MinimumLevel.Is(Enum.Parse<LogEventLevel>(defaultLevel, ignoreCase: true));

            loggerConfiguration.MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
            loggerConfiguration.MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning);
            loggerConfiguration.MinimumLevel.Override("Microsoft.EntityFrameworkCore.Query", LogEventLevel.Fatal);
            loggerConfiguration.MinimumLevel.Override("System", LogEventLevel.Warning);

            foreach (var exporter in enabledExporters)
            {
                try
                {
                    exporter.Configure(loggerConfiguration);
                }
                catch (Exception ex)
                {

                    Console.WriteLine($"Warning: Failed to configure log exporter '{exporter.Name}': {ex.Message}");
                }
            }

            loggerConfiguration
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.WithProperty("Application", "LoadlyAggregator.API")
                .Enrich.WithProperty("Environment", environment.EnvironmentName);
        });
        return hostBuilder;
    }
}
