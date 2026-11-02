using LoadlyAggregator.Domain.Admin.DTOs.Collectors;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Collectors;

public class BulkUpdateCollectorStatusAction
{
    private readonly ICollectorRepository _collectorRepository;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtTokenService _jwtTokenService;

    public BulkUpdateCollectorStatusAction(
        ICollectorRepository collectorRepository,
        IRefreshTokenService refreshTokenService,
        IJwtTokenService jwtTokenService)
    {
        _collectorRepository = collectorRepository;
        _refreshTokenService = refreshTokenService;
        _jwtTokenService = jwtTokenService;
    }

    public async Task ExecuteAsync(BulkUpdateCollectorStatusDto dto)
    {
        // Business logic: Validate and get existing IDs
        var existingIds = await _collectorRepository.GetExistingIdsAsync(dto.CollectorIds);

        if (existingIds.Count > 0)
        {
            // Business logic: Update status
            var updatedCount = await _collectorRepository.BulkUpdateStatusAsync(
                existingIds,
                (UserStatus)dto.Status);

            // Business logic: Handle token revocation
            await _refreshTokenService.HandleStatusChangeTokenRevocationAsync(
                existingIds,
                dto.Status,
                updatedCount,
                _jwtTokenService);
        }
    }
}
