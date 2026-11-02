namespace LoadlyAggregator.Domain.Common.Services.Mfa;

public class MfaMethodStrategyFactory
{
    private readonly Dictionary<string, IMfaMethodStrategy> _strategies;
    public MfaMethodStrategyFactory(IEnumerable<IMfaMethodStrategy> strategies)
    {
        _strategies = strategies.ToDictionary(s => s.Method, s => s);
    }
    public IMfaMethodStrategy GetStrategy(string method)
    {
        if (string.IsNullOrWhiteSpace(method))
            method = "email";
        if (!_strategies.TryGetValue(method, out var strategy))
            throw new ArgumentException($"Unsupported MFA method: {method}");
        return strategy;
    }
}
