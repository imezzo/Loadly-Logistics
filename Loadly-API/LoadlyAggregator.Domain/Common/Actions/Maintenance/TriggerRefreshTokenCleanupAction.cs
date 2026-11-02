using LoadlyAggregator.Domain.Common.Interfaces;
namespace LoadlyAggregator.Domain.Common.Actions.Maintenance;

public class TriggerRefreshTokenCleanupAction
{
    private readonly IRefreshTokenCleanupService _cleanupService;

    public TriggerRefreshTokenCleanupAction(IRefreshTokenCleanupService cleanupService)
    {
        _cleanupService = cleanupService;
    }

    public async Task<RefreshTokenCleanupResult> ExecuteAsync()
    {
        return await _cleanupService.PerformCleanupAsync();
    }
}
