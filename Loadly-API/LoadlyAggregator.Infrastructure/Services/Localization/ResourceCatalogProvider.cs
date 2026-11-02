using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace LoadlyAggregator.Infrastructure.Services.Localization;

public class ResourceCatalogProvider
{
    private readonly ILogger<ResourceCatalogProvider> _logger;
    private Dictionary<string, Dictionary<string, string>>? _catalog;
    private readonly object _lock = new();

    private static readonly string[] SupportedLanguages = ["en", "ar"];

    public ResourceCatalogProvider(ILogger<ResourceCatalogProvider> logger)
    {
        _logger = logger;
    }

    public Dictionary<string, Dictionary<string, string>> GetCatalog()
    {
        if (_catalog != null)
            return _catalog;

        lock (_lock)
        {
            if (_catalog != null)
                return _catalog;

            var loaded = LoadCatalog();
            _catalog = loaded;
            return _catalog;
        }
    }

    private Dictionary<string, Dictionary<string, string>> LoadCatalog()
    {
        var catalog = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

        var resourcesPath = ResolveResourcesPath();
        if (resourcesPath == null)
        {
            _logger.LogWarning("Resources folder not found for message catalogs");
            return catalog;
        }

        _logger.LogInformation("Found Resources folder at: {Path}", resourcesPath);

        foreach (var lang in SupportedLanguages)
        {
            var filePath = Path.Combine(resourcesPath, $"Messages.{lang}.json");
            if (!File.Exists(filePath))
            {
                _logger.LogWarning("Message file not found: {Path}", filePath);
                continue;
            }

            try
            {
                var json = File.ReadAllText(filePath);
                var root = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
                if (root != null)
                {
                    var flattened = FlattenJson(root);
                    catalog[lang] = flattened;
                    _logger.LogInformation("Loaded {Count} message keys for {Lang} from {Path}",
                        flattened.Count, lang, filePath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load message catalog from {Path}", filePath);
            }
        }

        return catalog;
    }

    private static string? ResolveResourcesPath()
    {
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var currentDir = Directory.GetCurrentDirectory();
        var assemblyLocation = Path.GetDirectoryName(typeof(ResourceCatalogProvider).Assembly.Location) ?? "";

        var candidates = new[]
        {
            Path.Combine(baseDir, "Resources"),
            Path.Combine(baseDir, "..", "..", "..", "..", "LoadlyAggregator.Shared", "Resources"),
            Path.Combine(currentDir, "Resources"),
            Path.Combine(currentDir, "..", "LoadlyAggregator.Shared", "Resources"),
            Path.Combine(assemblyLocation, "Resources"),
            Path.Combine(assemblyLocation, "..", "..", "..", "..", "LoadlyAggregator.Shared", "Resources")
        };

        foreach (var path in candidates)
        {
            if (Directory.Exists(path))
                return path;
        }

        return null;
    }

    private static Dictionary<string, string> FlattenJson(
        Dictionary<string, JsonElement> json, string prefix = "")
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var kvp in json)
        {
            var key = string.IsNullOrEmpty(prefix) ? kvp.Key : $"{prefix}.{kvp.Key}";

            if (kvp.Value.ValueKind == JsonValueKind.Object)
            {
                var nested = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(kvp.Value.GetRawText());
                if (nested != null)
                {
                    foreach (var child in FlattenJson(nested, key))
                    {
                        result[child.Key] = child.Value;
                    }
                }
            }
            else
            {
                result[key] = kvp.Value.GetString() ?? string.Empty;
            }
        }

        return result;
    }
}
