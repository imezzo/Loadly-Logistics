using LoadlyAggregator.Domain.Common.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Domain.Company.DTOs.Auth;
using LoadlyAggregator.Shared.Exceptions;
namespace LoadlyAggregator.Domain.Company.Actions.Auth;



public class CompanyChangePasswordAction : IAction<CompanyChangePasswordDto, bool>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtTokenService _jwtTokenService;
    public CompanyChangePasswordAction(
        ICompanyRepository companyRepository,
        IPasswordHashingService passwordHashingService,
        IRefreshTokenService refreshTokenService,
        IJwtTokenService jwtTokenService)
    {
        _companyRepository = companyRepository;
        _passwordHashingService = passwordHashingService;
        _refreshTokenService = refreshTokenService;
        _jwtTokenService = jwtTokenService;
    }
    public async Task<bool> ExecuteAsync(CompanyChangePasswordDto dto)
    {
        var company = await _companyRepository.GetByIdAsync(dto.UserId);
        UserValidator.ValidateUserExists(company, "Company");
        UserValidator.ValidatePassword(_passwordHashingService, dto.CurrentPassword, company!.Password);
        UpdatePassword(company, dto.NewPassword);
        await _companyRepository.UpdateAsync(company);
        await Task.WhenAll(
            _refreshTokenService.RevokeModelTokensAsync(company.Id, "Company"),
            _jwtTokenService.RevokeAllUserTokensAsync(company.Id, "company", expirationMinutes: 120));
        return true;
    }
    private void UpdatePassword(Domain.Common.Entities.Company company, string newPassword)
    {
        company.Password = _passwordHashingService.HashPassword(newPassword);
    }
}
