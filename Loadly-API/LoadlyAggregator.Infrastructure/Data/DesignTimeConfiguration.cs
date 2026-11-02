using Microsoft.Extensions.Configuration;

namespace LoadlyAggregator.Infrastructure.Data;

internal static class DesignTimeConfiguration
{
    internal static IConfigurationRoot Build()
    {
        var builder = new ConfigurationBuilder()
            .AddEnvironmentVariables();

        var basePath = FindAppsettingsDirectory();
        if (basePath != null)
        {
            builder
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true);
        }

        return builder.Build();
    }

    private static string? FindAppsettingsDirectory()
    {
        string[] candidates =
        [
            Directory.GetCurrentDirectory(),
            AppContext.BaseDirectory,
            Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../LoadlyAggregator.API")),
            "/app",
        ];

        foreach (var candidate in candidates.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (File.Exists(Path.Combine(candidate, "appsettings.json")))
                return candidate;
        }

        return null;
    }
}
