using LoadlyAggregator.Domain.Common.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Exceptions;
namespace LoadlyAggregator.Domain.Contractor.Actions.Auth;




public class ContractorRefreshTokenAction
{
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IContractorRepository _contractorRepository;
    private readonly IPermissionService _permissionService;
    public ContractorRefreshTokenAction(
        IRefreshTokenService refreshTokenService,
        IJwtTokenService jwtTokenService,
        IContractorRepository contractorRepository,
        IPermissionService permissionService)
    {
        _refreshTokenService = refreshTokenService;
        _jwtTokenService = jwtTokenService;
        _contractorRepository = contractorRepository;
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
        _ = storedToken.ModelType == "Contractor"
            ? true
            : throw new UnauthorizedException("auth.invalid_refresh_token_for_contractor");
        var contractor = await _contractorRepository.GetByIdAsync(storedToken.ModelId);
        UserValidator.ValidateUserExists(contractor, "Contractor");
        UserValidator.ValidateStatus(contractor!.Status);
        var (newRefreshTokenEntity, newPlainRefreshToken) = await _refreshTokenService.RotateRefreshTokenAsync(storedToken);
        return new
        {
            AccessToken = _jwtTokenService.GenerateToken(
                contractor.Id,
                "contractor",
                contractor.Email,
                roles: null,
                permissions: null,
                tokenFamilyId: newRefreshTokenEntity.TokenFamilyId),
            RefreshToken = newPlainRefreshToken,
            ExpiresIn = 3600,
            User = new
            {
                Id = contractor.Id,
                FullName = contractor.FullName,
                UserName = contractor.UserName,
                Email = contractor.Email,
                Phone = contractor.Phone,
                UserType = "contractor",
                MfaChannel = (int)contractor.MfaChannel,
                Permissions = await _permissionService.GetPermissionsGroupedByGroupAsync(contractor)
            }
        };
    }
}
