using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.Auth;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Company.Actions.Auth;






public class CompanyResetPasswordWithCodeAction : IAction<CompanyResetPasswordWithCodeDto, bool>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly IPasswordResetCacheService _passwordResetCacheService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public CompanyResetPasswordWithCodeAction(
        ICompanyRepository companyRepository,
        IPasswordHashingService passwordHashingService,
        IPasswordResetCacheService passwordResetCacheService,
        IRefreshTokenService refreshTokenService,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _companyRepository = companyRepository;
        _passwordHashingService = passwordHashingService;
        _passwordResetCacheService = passwordResetCacheService;
        _refreshTokenService = refreshTokenService;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> ExecuteAsync(CompanyResetPasswordWithCodeDto dto)
    {
        _ = dto.NewPassword == dto.ConfirmPassword
            ? true
            : throw new ValidationException("auth.password_mismatch");
        _ = await _passwordResetCacheService.IsRateLimitedAsync("company", dto.Identifier)
            ? throw new TooManyRequestsException("auth.too_many_failed_attempts")
            : true;
        var resetAttempt = await _passwordResetCacheService.GetResetAttemptAsync("company", dto.Identifier);
        _ = resetAttempt != null
            ? true
            : await HandleNullResetAttemptAsync(dto.Identifier);
        _ = resetAttempt!.Code.Equals(dto.Code, StringComparison.OrdinalIgnoreCase)
            ? true
            : await HandleInvalidResetCodeAsync(dto.Identifier, resetAttempt);
        var company = await _companyRepository.GetByIdAsync(resetAttempt.UserId);
        _ = company != null
            ? true
            : await HandleCompanyNotFoundAsync(dto.Identifier);
        _ = !string.IsNullOrWhiteSpace(dto.NewPassword) && dto.NewPassword.Length >= 6
            ? true
            : throw new ValidationException("auth.password_min_length");
        company!.Password = _passwordHashingService.HashPassword(dto.NewPassword);
        company.EmailVerified = resetAttempt.Channel == CommunicationChannel.Email ? true : company.EmailVerified;
        company.PhoneVerified = resetAttempt.Channel == CommunicationChannel.Sms ? true : company.PhoneVerified;
        await _companyRepository.UpdateAsync(company);
        await _unitOfWork.SaveChangesAsync();
        await _passwordResetCacheService.DeleteResetAttemptAsync("company", dto.Identifier);
        await Task.WhenAll(
            _refreshTokenService.RevokeModelTokensAsync(company.Id, "Company"),
            _jwtTokenService.RevokeAllUserTokensAsync(company.Id, "company", expirationMinutes: 120));
        return true;
    }

    private async Task<bool> HandleNullResetAttemptAsync(string identifier)
    {
        await _passwordResetCacheService.IncrementFailedAttemptsAsync("company", identifier);
        throw new UnauthorizedException("auth.invalid_or_expired_reset_code");
    }

    private async Task<bool> HandleInvalidResetCodeAsync(string identifier, Domain.Common.Interfaces.PasswordResetAttempt resetAttempt)
    {
        await _passwordResetCacheService.IncrementFailedAttemptsAsync("company", identifier);
        await (resetAttempt.FailedAttempts >= 4
            ? _passwordResetCacheService.DeleteResetAttemptAsync("company", identifier)
            : Task.CompletedTask);
        throw new UnauthorizedException("auth.invalid_or_expired_reset_code");
    }

    private async Task<bool> HandleCompanyNotFoundAsync(string identifier)
    {
        await _passwordResetCacheService.DeleteResetAttemptAsync("company", identifier);
        throw new NotFoundException("auth.user_not_found");
    }
}

