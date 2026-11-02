using LoadlyAggregator.Domain.Common.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Exceptions;
namespace LoadlyAggregator.Domain.Company.Actions.Auth;




public class CompanyRefreshTokenAction
{
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ICompanyRepository _companyRepository;
    private readonly IPermissionService _permissionService;
    public CompanyRefreshTokenAction(
        IRefreshTokenService refreshTokenService,
        IJwtTokenService jwtTokenService,
        ICompanyRepository companyRepository,
        IPermissionService permissionService)
    {
        _refreshTokenService = refreshTokenService;
        _jwtTokenService = jwtTokenService;
        _companyRepository = companyRepository;
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
        _ = storedToken.ModelType == "Company"
            ? true
            : throw new UnauthorizedException("auth.invalid_refresh_token_for_company");
        var company = await _companyRepository.GetByIdAsync(storedToken.ModelId);
        UserValidator.ValidateUserExists(company, "Company");
        UserValidator.ValidateStatus(company!.Status);
        var (newRefreshTokenEntity, newPlainRefreshToken) = await _refreshTokenService.RotateRefreshTokenAsync(storedToken);
        return new
        {
            AccessToken = _jwtTokenService.GenerateToken(
                company.Id,
                "company",
                company.Email,
                roles: null,
                permissions: null,
                tokenFamilyId: newRefreshTokenEntity.TokenFamilyId,
                companyId: company.CompanyId ?? company.Id),
            RefreshToken = newPlainRefreshToken,
            ExpiresIn = 3600,
            User = new
            {
                Id = company.Id,
                FullName = company.FullName,
                UserName = company.UserName,
                Email = company.Email,
                Phone = company.Phone,
                UserType = "company",
                MfaChannel = (int)company.MfaChannel,
                Permissions = await _permissionService.GetPermissionsGroupedByGroupAsync(company)
            }
        };
    }
}
