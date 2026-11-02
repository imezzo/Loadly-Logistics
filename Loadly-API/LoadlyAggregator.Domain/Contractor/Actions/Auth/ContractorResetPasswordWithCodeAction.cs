using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Contractor.DTOs.Auth;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Contractor.Actions.Auth;






public class ContractorResetPasswordWithCodeAction : IAction<ContractorResetPasswordWithCodeDto, bool>
{
    private readonly IContractorRepository _contractorRepository;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly IPasswordResetCacheService _passwordResetCacheService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public ContractorResetPasswordWithCodeAction(
        IContractorRepository contractorRepository,
        IPasswordHashingService passwordHashingService,
        IPasswordResetCacheService passwordResetCacheService,
        IRefreshTokenService refreshTokenService,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _contractorRepository = contractorRepository;
        _passwordHashingService = passwordHashingService;
        _passwordResetCacheService = passwordResetCacheService;
        _refreshTokenService = refreshTokenService;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> ExecuteAsync(ContractorResetPasswordWithCodeDto dto)
    {
        _ = dto.NewPassword == dto.ConfirmPassword
            ? true
            : throw new ValidationException("auth.password_mismatch");
        _ = await _passwordResetCacheService.IsRateLimitedAsync("contractor", dto.Identifier)
            ? throw new TooManyRequestsException("auth.too_many_failed_attempts")
            : true;
        var resetAttempt = await _passwordResetCacheService.GetResetAttemptAsync("contractor", dto.Identifier);
        _ = resetAttempt != null
            ? true
            : await HandleNullResetAttemptAsync(dto.Identifier);
        _ = resetAttempt!.Code.Equals(dto.Code, StringComparison.OrdinalIgnoreCase)
            ? true
            : await HandleInvalidResetCodeAsync(dto.Identifier, resetAttempt);
        var contractor = await _contractorRepository.GetByIdAsync(resetAttempt.UserId);
        _ = contractor != null
            ? true
            : await HandleContractorNotFoundAsync(dto.Identifier);
        _ = !string.IsNullOrWhiteSpace(dto.NewPassword) && dto.NewPassword.Length >= 6
            ? true
            : throw new ValidationException("auth.password_min_length");
        contractor!.Password = _passwordHashingService.HashPassword(dto.NewPassword);
        contractor.EmailVerified = resetAttempt.Channel == CommunicationChannel.Email ? true : contractor.EmailVerified;
        contractor.PhoneVerified = resetAttempt.Channel == CommunicationChannel.Sms ? true : contractor.PhoneVerified;
        await _contractorRepository.UpdateAsync(contractor);
        await _unitOfWork.SaveChangesAsync();
        await _passwordResetCacheService.DeleteResetAttemptAsync("contractor", dto.Identifier);
        await Task.WhenAll(
            _refreshTokenService.RevokeModelTokensAsync(contractor.Id, "Contractor"),
            _jwtTokenService.RevokeAllUserTokensAsync(contractor.Id, "contractor", expirationMinutes: 120));
        return true;
    }

    private async Task<bool> HandleNullResetAttemptAsync(string identifier)
    {
        await _passwordResetCacheService.IncrementFailedAttemptsAsync("contractor", identifier);
        throw new UnauthorizedException("auth.invalid_or_expired_reset_code");
    }

    private async Task<bool> HandleInvalidResetCodeAsync(string identifier, Domain.Common.Interfaces.PasswordResetAttempt resetAttempt)
    {
        await _passwordResetCacheService.IncrementFailedAttemptsAsync("contractor", identifier);
        await (resetAttempt.FailedAttempts >= 4
            ? _passwordResetCacheService.DeleteResetAttemptAsync("contractor", identifier)
            : Task.CompletedTask);
        throw new UnauthorizedException("auth.invalid_or_expired_reset_code");
    }

    private async Task<bool> HandleContractorNotFoundAsync(string identifier)
    {
        await _passwordResetCacheService.DeleteResetAttemptAsync("contractor", identifier);
        throw new NotFoundException("auth.user_not_found");
    }
}

