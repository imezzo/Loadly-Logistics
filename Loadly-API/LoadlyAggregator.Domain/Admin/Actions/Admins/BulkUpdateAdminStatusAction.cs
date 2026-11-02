using LoadlyAggregator.Domain.Admin.DTOs.Admins;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Admins;

public class BulkUpdateAdminStatusAction
{
    private readonly IAdminRepository _adminRepository;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtTokenService _jwtTokenService;

    public BulkUpdateAdminStatusAction(
        IAdminRepository adminRepository,
        IRefreshTokenService refreshTokenService,
        IJwtTokenService jwtTokenService)
    {
        _adminRepository = adminRepository;
        _refreshTokenService = refreshTokenService;
        _jwtTokenService = jwtTokenService;
    }

    public async Task ExecuteAsync(BulkUpdateAdminStatusDto dto)
    {
        // Business logic: Validate and get existing IDs
        var existingIds = await _adminRepository.GetExistingIdsAsync(dto.AdminIds);

        if (existingIds.Count > 0)
        {
            // Business logic: Update status
            var updatedCount = await _adminRepository.BulkUpdateStatusAsync(
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
