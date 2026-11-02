using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.File;
using LoadlyAggregator.Domain.Common.Interfaces;
namespace LoadlyAggregator.Infrastructure.Services.Logging.Exporters;



public class FileLogExporter : ILogExporter
{
    private readonly IConfiguration _configuration;
    public FileLogExporter(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public string Name => "file";
    public void Configure(object loggerConfiguration)
    {

        if (loggerConfiguration is LoggerConfiguration config)
        {
            var path = _configuration["Serilog:File:Path"] ?? "logs/app-.log";
            var retainedFileCountLimit = _configuration.GetValue<int>("Serilog:File:RetainedFileCountLimit", 30);
            var fileSizeLimitBytes = _configuration.GetValue<long>("Serilog:File:FileSizeLimitBytes", 10485760);
            config.WriteTo.Async(a => a.File(
                path: path,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: retainedFileCountLimit,
                fileSizeLimitBytes: fileSizeLimitBytes,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj} {Properties:j}{NewLine}{Exception}",
                shared: true,
                flushToDiskInterval: TimeSpan.FromSeconds(1)
            ));
        }
    }
    public bool IsEnabled(string[] exporters, Dictionary<string, object> settings)
    {
        return exporters.Contains(Name, StringComparer.OrdinalIgnoreCase) ||
               (exporters.Length == 0 && (!settings.ContainsKey("File:Enabled") ||
                (settings["File:Enabled"] is bool enabled && enabled)));
    }
}
