using LoadlyAggregator.Domain.Common.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Exceptions;
namespace LoadlyAggregator.Domain.Admin.Actions.Auth;




public class AdminRefreshTokenAction
{
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IAdminRepository _adminRepository;
    private readonly IPermissionService _permissionService;
    public AdminRefreshTokenAction(
        IRefreshTokenService refreshTokenService,
        IJwtTokenService jwtTokenService,
        IAdminRepository adminRepository,
        IPermissionService permissionService)
    {
        _refreshTokenService = refreshTokenService;
        _jwtTokenService = jwtTokenService;
        _adminRepository = adminRepository;
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
        _ = storedToken.ModelType == "Admin"
            ? true
            : throw new UnauthorizedException("auth.invalid_refresh_token_for_driver");
        var admin = await _adminRepository.GetByIdAsync(storedToken.ModelId);
        UserValidator.ValidateUserExists(admin, "Admin");
        UserValidator.ValidateStatus(admin!.Status);
        var (newRefreshTokenEntity, newPlainRefreshToken) = await _refreshTokenService.RotateRefreshTokenAsync(storedToken);
        return new
        {
            AccessToken = _jwtTokenService.GenerateToken(
                admin.Id,
                "admin",
                admin.Email,
                roles: null,
                permissions: null,
                tokenFamilyId: newRefreshTokenEntity.TokenFamilyId),
            RefreshToken = newPlainRefreshToken,
            ExpiresIn = 3600,
            User = new
            {
                Id = admin.Id,
                FullName = admin.FullName,
                UserName = admin.UserName,
                Email = admin.Email,
                Phone = admin.Phone,
                UserType = "admin",
                MfaChannel = (int)admin.MfaChannel,
                Permissions = await _permissionService.GetPermissionsGroupedByGroupAsync(admin)
            }
        };
    }
}
