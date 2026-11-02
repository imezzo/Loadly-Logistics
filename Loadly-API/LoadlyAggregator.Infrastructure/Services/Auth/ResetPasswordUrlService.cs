using Microsoft.Extensions.Configuration;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Services.Auth;





public class ResetPasswordUrlService : IResetPasswordUrlService
{
    private readonly IConfiguration _configuration;
    private readonly Dictionary<string, string> _baseUrls;

    public ResetPasswordUrlService(IConfiguration configuration)
    {
        _configuration = configuration;
        _baseUrls = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);


        LoadBaseUrl("admin");
        LoadBaseUrl("driver");
        LoadBaseUrl("company");
        LoadBaseUrl("contractor");
        LoadBaseUrl("collector");
    }

    private void LoadBaseUrl(string module)
    {
        var configKey = $"ResetPasswordUrls:{module}:BaseUrl";
        var baseUrl = _configuration[configKey];

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new InvalidOperationException(
                $"Reset password base URL for module '{module}' not configured. " +
                $"Please add '{configKey}' to appsettings.json");
        }


        baseUrl = baseUrl.TrimEnd('/');
        _baseUrls[module.ToLower()] = baseUrl;
    }

    public string GetBaseUrl(string module)
    {
        var normalizedModule = module.ToLower();
        if (!_baseUrls.TryGetValue(normalizedModule, out var baseUrl))
        {
            throw new LogicalException("auth.unsupported_module_for_reset_url", module, "admin, driver, company, contractor, collector");
        }

        return baseUrl;
    }

    public string GenerateResetPasswordUrl(string module, string token)
    {
        var baseUrl = GetBaseUrl(module);
        var normalizedModule = module.ToLower();


        var pathConfigKey = $"ResetPasswordUrls:{normalizedModule}:Path";
        var path = _configuration[pathConfigKey] ?? "/auth/reset-password";


        path = path.TrimStart('/');


        return $"{baseUrl}/{path}?token={token}";
    }
}

