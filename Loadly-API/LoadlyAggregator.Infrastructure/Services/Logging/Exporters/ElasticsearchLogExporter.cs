using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using LoadlyAggregator.Domain.Common.Interfaces;
namespace LoadlyAggregator.Infrastructure.Services.Logging.Exporters;



public class ElasticsearchLogExporter : ILogExporter
{
    private readonly IConfiguration _configuration;
    public ElasticsearchLogExporter(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public string Name => "elasticsearch";
    public void Configure(object loggerConfiguration)
    {

        if (loggerConfiguration is LoggerConfiguration config)
        {
            var nodeUris = _configuration["Serilog:Elasticsearch:NodeUris"]
                ?? Environment.GetEnvironmentVariable("ELASTICSEARCH_NODES")
                ?? "http://localhost:9200";

            var indexFormat = _configuration["Serilog:Elasticsearch:IndexFormat"]
                ?? "loadly-aggregator-logs-{0:yyyy.MM.dd}";

            config.WriteTo.Elasticsearch(
                new ElasticsearchSinkOptions(new Uri(nodeUris))
                {
                    AutoRegisterTemplate = true,
                    IndexFormat = indexFormat,
                    NumberOfShards = 1,
                    NumberOfReplicas = 1
                }
            );
        }
    }
    public bool IsEnabled(string[] exporters, Dictionary<string, object> settings)
    {
        if (!exporters.Contains(Name, StringComparer.OrdinalIgnoreCase))
        {
            return false;
        }

        var nodeUris = _configuration["Serilog:Elasticsearch:NodeUris"]
            ?? Environment.GetEnvironmentVariable("ELASTICSEARCH_NODES");

        return !string.IsNullOrWhiteSpace(nodeUris);
    }
}
