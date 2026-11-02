using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Domain.Company.DTOs.Auth;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Company.Actions.Auth;


public class CompanyResetPasswordAction : IAction<CompanyResetPasswordDto, bool>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly IPasswordResetTokenService _passwordResetTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;
    public CompanyResetPasswordAction(
        ICompanyRepository companyRepository,
        IPasswordHashingService passwordHashingService,
        IPasswordResetTokenService passwordResetTokenService,
        IRefreshTokenService refreshTokenService,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _companyRepository = companyRepository;
        _passwordHashingService = passwordHashingService;
        _passwordResetTokenService = passwordResetTokenService;
        _refreshTokenService = refreshTokenService;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }
    public async Task<bool> ExecuteAsync(CompanyResetPasswordDto dto)
    {
        ValidatePasswordsMatch(dto);
        ValidatePasswordStrength(dto.NewPassword);
        var tokenData = _passwordResetTokenService.ValidateResetToken(dto.Token)
            ?? throw new UnauthorizedException("auth.invalid_or_expired_reset_token");
        _ = tokenData.UserType == "company"
            ? true
            : throw new UnauthorizedException("auth.invalid_token_for_company");
        var company = await _companyRepository.GetByIdAsync(tokenData.UserId);
        UserValidator.ValidateUserExists(company, "Company");
        UpdatePassword(company!, dto.NewPassword);
        VerifyChannelBasedOnToken(company!, tokenData.Channel);
        await _companyRepository.UpdateAsync(company!);
        await _unitOfWork.SaveChangesAsync();
        await Task.WhenAll(
            _refreshTokenService.RevokeModelTokensAsync(company!.Id, "Company"),
            _jwtTokenService.RevokeAllUserTokensAsync(company!.Id, "company", expirationMinutes: 120));
        return true;
    }
    private static void ValidatePasswordsMatch(CompanyResetPasswordDto dto)
    {
        _ = dto.NewPassword == dto.ConfirmPassword
            ? true
            : throw new ValidationException("auth.password_mismatch");
    }
    private static void ValidatePasswordStrength(string password)
    {
        _ = !string.IsNullOrWhiteSpace(password) && password.Length >= 6
            ? true
            : throw new ValidationException("auth.password_min_length");
    }
    private void UpdatePassword(Domain.Common.Entities.Company company, string newPassword)
    {
        company.Password = _passwordHashingService.HashPassword(newPassword);
    }
    private void VerifyChannelBasedOnToken(Domain.Common.Entities.Company company, CommunicationChannel channel)
    {
        company.EmailVerified = channel == CommunicationChannel.Email ? true : company.EmailVerified;
        company.PhoneVerified = channel == CommunicationChannel.Sms ? true : company.PhoneVerified;
    }
}
