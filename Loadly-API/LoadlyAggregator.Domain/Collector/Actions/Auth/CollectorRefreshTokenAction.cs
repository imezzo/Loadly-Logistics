using LoadlyAggregator.Domain.Common.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Exceptions;
namespace LoadlyAggregator.Domain.Collector.Actions.Auth;




public class CollectorRefreshTokenAction
{
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ICollectorRepository _collectorRepository;
    private readonly IPermissionService _permissionService;
    public CollectorRefreshTokenAction(
        IRefreshTokenService refreshTokenService,
        IJwtTokenService jwtTokenService,
        ICollectorRepository collectorRepository,
        IPermissionService permissionService)
    {
        _refreshTokenService = refreshTokenService;
        _jwtTokenService = jwtTokenService;
        _collectorRepository = collectorRepository;
        _permissionService = permissionService;
    }
    public async Task<object> ExecuteAsync(RefreshTokenRequestDto dto)
    {
        var validationResult = await _refreshTokenService.ValidateRefreshTokenAsync(dto.RefreshToken);
        _ = validationResult.IsValid
            ? true
            : throw new UnauthorizedException(
                validationResult.IsTheftDetected
                    ? "Security alert: Token theft detected. Please login again."
                    : validationResult.ErrorMessage ?? "Invalid refresh token");
        var storedToken = validationResult.Token!;
        _ = storedToken.ModelType == "Collector"
            ? true
            : throw new UnauthorizedException("auth.invalid_refresh_token_for_driver");
        var collector = await _collectorRepository.GetByIdAsync(storedToken.ModelId);
        UserValidator.ValidateUserExists(collector, "Collector");
        UserValidator.ValidateStatus(collector!.Status);
        var (newRefreshTokenEntity, newPlainRefreshToken) = await _refreshTokenService.RotateRefreshTokenAsync(storedToken);
        return new
        {
            AccessToken = _jwtTokenService.GenerateToken(
                collector.Id,
                "collector",
                collector.Email,
                roles: null,
                permissions: null,
                tokenFamilyId: newRefreshTokenEntity.TokenFamilyId),
            RefreshToken = newPlainRefreshToken,
            ExpiresIn = 3600,
            User = new
            {
                Id = collector.Id,
                FullName = collector.FullName,
                UserName = collector.UserName,
                Email = collector.Email,
                Phone = collector.Phone,
                UserType = "collector",
                MfaChannel = (int)collector.MfaChannel,
                Permissions = await _permissionService.GetPermissionsGroupedByGroupAsync(collector)
            }
        };
    }
}
