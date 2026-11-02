using Microsoft.Extensions.Configuration;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Services.Logging.Exporters;

namespace LoadlyAggregator.Infrastructure.Services.Logging;






public static class LogExporterFactory
{




    public static IEnumerable<ILogExporter> GetEnabledExporters(IConfiguration configuration)
    {






        var exporters = new List<ILogExporter>
        {
            new ConsoleLogExporter(),
            new FileLogExporter(configuration),
            new SeqLogExporter(configuration),
            new ElasticsearchLogExporter(configuration)
        };


        var enabledExporters = configuration.GetSection("Serilog:Exporters").Get<string[]>() ?? Array.Empty<string>();
        var settings = new Dictionary<string, object>();


        settings["Console:Enabled"] = configuration.GetValue<bool>("Serilog:Console:Enabled", true);
        settings["File:Enabled"] = configuration.GetValue<bool>("Serilog:File:Enabled", true);

        return exporters.Where(e => e.IsEnabled(enabledExporters, settings));
    }
}

