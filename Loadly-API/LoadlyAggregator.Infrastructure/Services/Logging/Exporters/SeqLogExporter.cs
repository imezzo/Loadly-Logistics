using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.Seq;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Infrastructure.Services.Logging.Exporters;




public class SeqLogExporter : ILogExporter
{
    private readonly IConfiguration _configuration;

    public SeqLogExporter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string Name => "seq";

    public void Configure(object loggerConfiguration)
    {

        if (loggerConfiguration is LoggerConfiguration config)
        {
            var serverUrl = _configuration["Serilog:Seq:ServerUrl"]
                ?? Environment.GetEnvironmentVariable("SEQ_SERVER_URL")
                ?? "http://localhost:5341";

            var apiKey = _configuration["Serilog:Seq:ApiKey"]
                ?? Environment.GetEnvironmentVariable("SEQ_API_KEY");

            config.WriteTo.Seq(
                serverUrl: serverUrl,
                apiKey: apiKey
            );
        }
    }

    public bool IsEnabled(string[] exporters, Dictionary<string, object> settings)
    {
        if (!exporters.Contains(Name, StringComparer.OrdinalIgnoreCase))
        {
            return false;
        }


        var serverUrl = _configuration["Serilog:Seq:ServerUrl"]
            ?? Environment.GetEnvironmentVariable("SEQ_SERVER_URL");

        return !string.IsNullOrWhiteSpace(serverUrl);
    }
}

