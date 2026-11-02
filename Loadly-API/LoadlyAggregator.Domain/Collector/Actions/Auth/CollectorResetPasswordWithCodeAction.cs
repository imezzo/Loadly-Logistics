using LoadlyAggregator.Domain.Collector.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Collector.Actions.Auth;






public class CollectorResetPasswordWithCodeAction : IAction<CollectorResetPasswordWithCodeDto, bool>
{
    private readonly ICollectorRepository _collectorRepository;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly IPasswordResetCacheService _passwordResetCacheService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public CollectorResetPasswordWithCodeAction(
        ICollectorRepository collectorRepository,
        IPasswordHashingService passwordHashingService,
        IPasswordResetCacheService passwordResetCacheService,
        IRefreshTokenService refreshTokenService,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _collectorRepository = collectorRepository;
        _passwordHashingService = passwordHashingService;
        _passwordResetCacheService = passwordResetCacheService;
        _refreshTokenService = refreshTokenService;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> ExecuteAsync(CollectorResetPasswordWithCodeDto dto)
    {
        _ = dto.NewPassword == dto.ConfirmPassword
            ? true
            : throw new ValidationException("auth.password_mismatch");
        _ = await _passwordResetCacheService.IsRateLimitedAsync("collector", dto.Identifier)
            ? throw new TooManyRequestsException("auth.too_many_failed_attempts")
            : true;
        var resetAttempt = await _passwordResetCacheService.GetResetAttemptAsync("collector", dto.Identifier);
        _ = resetAttempt != null
            ? true
            : await HandleNullResetAttemptAsync(dto.Identifier);
        _ = resetAttempt!.Code.Equals(dto.Code, StringComparison.OrdinalIgnoreCase)
            ? true
            : await HandleInvalidResetCodeAsync(dto.Identifier, resetAttempt);
        var collector = await _collectorRepository.GetByIdAsync(resetAttempt.UserId);
        _ = collector != null
            ? true
            : await HandleCollectorNotFoundAsync(dto.Identifier);
        _ = !string.IsNullOrWhiteSpace(dto.NewPassword) && dto.NewPassword.Length >= 6
            ? true
            : throw new ValidationException("auth.password_min_length");
        collector!.Password = _passwordHashingService.HashPassword(dto.NewPassword);
        collector.EmailVerified = resetAttempt.Channel == CommunicationChannel.Email ? true : collector.EmailVerified;
        collector.PhoneVerified = resetAttempt.Channel == CommunicationChannel.Sms ? true : collector.PhoneVerified;
        await _collectorRepository.UpdateAsync(collector);
        await _unitOfWork.SaveChangesAsync();
        await _passwordResetCacheService.DeleteResetAttemptAsync("collector", dto.Identifier);
        await Task.WhenAll(
            _refreshTokenService.RevokeModelTokensAsync(collector.Id, "Collector"),
            _jwtTokenService.RevokeAllUserTokensAsync(collector.Id, "collector", expirationMinutes: 120));
        return true;
    }

    private async Task<bool> HandleNullResetAttemptAsync(string identifier)
    {
        await _passwordResetCacheService.IncrementFailedAttemptsAsync("collector", identifier);
        throw new UnauthorizedException("auth.invalid_or_expired_reset_code");
    }

    private async Task<bool> HandleInvalidResetCodeAsync(string identifier, Domain.Common.Interfaces.PasswordResetAttempt resetAttempt)
    {
        await _passwordResetCacheService.IncrementFailedAttemptsAsync("collector", identifier);
        await (resetAttempt.FailedAttempts >= 4
            ? _passwordResetCacheService.DeleteResetAttemptAsync("collector", identifier)
            : Task.CompletedTask);
        throw new UnauthorizedException("auth.invalid_or_expired_reset_code");
    }

    private async Task<bool> HandleCollectorNotFoundAsync(string identifier)
    {
        await _passwordResetCacheService.DeleteResetAttemptAsync("collector", identifier);
        throw new NotFoundException("auth.user_not_found");
    }
}

