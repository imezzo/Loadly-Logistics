using Serilog;
using LoadlyAggregator.Domain.Common.Interfaces;
namespace LoadlyAggregator.Infrastructure.Services.Logging.Exporters;



public class ConsoleLogExporter : ILogExporter
{
    public string Name => "console";
    public void Configure(object loggerConfiguration)
    {

        if (loggerConfiguration is LoggerConfiguration config)
        {
            config.WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj} {Properties:j}{NewLine}{Exception}"
            );
        }
    }
    public bool IsEnabled(string[] exporters, Dictionary<string, object> settings)
    {
        return exporters.Contains(Name, StringComparer.OrdinalIgnoreCase) ||
               (exporters.Length == 0 && (!settings.ContainsKey("Console:Enabled") ||
                (settings["Console:Enabled"] is bool enabled && enabled)));
    }
}
