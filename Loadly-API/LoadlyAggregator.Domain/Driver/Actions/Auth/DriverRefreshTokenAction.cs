using LoadlyAggregator.Domain.Common.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Exceptions;
namespace LoadlyAggregator.Domain.Driver.Actions.Auth;




public class DriverRefreshTokenAction
{
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IDriverRepository _driverRepository;
    private readonly IPermissionService _permissionService;
    public DriverRefreshTokenAction(
        IRefreshTokenService refreshTokenService,
        IJwtTokenService jwtTokenService,
        IDriverRepository driverRepository,
        IPermissionService permissionService)
    {
        _refreshTokenService = refreshTokenService;
        _jwtTokenService = jwtTokenService;
        _driverRepository = driverRepository;
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
        _ = storedToken.ModelType == "Driver"
            ? true
            : throw new UnauthorizedException("auth.invalid_refresh_token_for_driver");
        var driver = await _driverRepository.GetByIdAsync(storedToken.ModelId);
        UserValidator.ValidateUserExists(driver, "Driver");
        UserValidator.ValidateStatus(driver!.Status);
        var (newRefreshTokenEntity, newPlainRefreshToken) = await _refreshTokenService.RotateRefreshTokenAsync(storedToken);
        return new
        {
            AccessToken = _jwtTokenService.GenerateToken(
                driver.Id,
                "driver",
                driver.Email,
                roles: null,
                permissions: null,
                tokenFamilyId: newRefreshTokenEntity.TokenFamilyId),
            RefreshToken = newPlainRefreshToken,
            ExpiresIn = 3600,
            User = new
            {
                Id = driver.Id,
                FullName = driver.FullName,
                UserName = driver.UserName,
                Email = driver.Email,
                Phone = driver.Phone,
                UserType = "driver",
                MfaChannel = (int)driver.MfaChannel,
                Permissions = await _permissionService.GetPermissionsGroupedByGroupAsync(driver)
            }
        };
    }
}
