using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Driver.DTOs.Auth;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Driver.Actions.Auth;






public class DriverResetPasswordWithCodeAction : IAction<DriverResetPasswordWithCodeDto, bool>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly IPasswordResetCacheService _passwordResetCacheService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public DriverResetPasswordWithCodeAction(
        IDriverRepository driverRepository,
        IPasswordHashingService passwordHashingService,
        IPasswordResetCacheService passwordResetCacheService,
        IRefreshTokenService refreshTokenService,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _driverRepository = driverRepository;
        _passwordHashingService = passwordHashingService;
        _passwordResetCacheService = passwordResetCacheService;
        _refreshTokenService = refreshTokenService;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> ExecuteAsync(DriverResetPasswordWithCodeDto dto)
    {
        _ = dto.NewPassword == dto.ConfirmPassword
            ? true
            : throw new ValidationException("auth.password_mismatch");

        var normalizedIdentifier = (dto.Identifier ?? string.Empty).Trim();
        _ = !string.IsNullOrWhiteSpace(normalizedIdentifier)
            ? true
            : throw new ValidationException("validation.identifier_required");

        _ = await _passwordResetCacheService.IsRateLimitedAsync("driver", normalizedIdentifier)
            ? throw new TooManyRequestsException("auth.too_many_failed_attempts")
            : true;
        var resetAttempt = await _passwordResetCacheService.GetResetAttemptAsync("driver", normalizedIdentifier);
        _ = resetAttempt != null
            ? true
            : await HandleNullResetAttemptAsync(normalizedIdentifier);
        _ = resetAttempt!.Code.Equals(dto.Code, StringComparison.OrdinalIgnoreCase)
            ? true
            : await HandleInvalidResetCodeAsync(normalizedIdentifier, resetAttempt);
        var driver = await _driverRepository.GetByIdAsync(resetAttempt.UserId);
        _ = driver != null
            ? true
            : await HandleDriverNotFoundAsync(normalizedIdentifier);
        _ = !string.IsNullOrWhiteSpace(dto.NewPassword) && dto.NewPassword.Length >= 6
            ? true
            : throw new ValidationException("auth.password_min_length");
        driver!.Password = _passwordHashingService.HashPassword(dto.NewPassword);
        driver.EmailVerified = resetAttempt.Channel == CommunicationChannel.Email ? true : driver.EmailVerified;
        driver.PhoneVerified = resetAttempt.Channel == CommunicationChannel.Sms ? true : driver.PhoneVerified;
        await _driverRepository.UpdateAsync(driver);
        await _unitOfWork.SaveChangesAsync();
        await _passwordResetCacheService.DeleteResetAttemptAsync("driver", normalizedIdentifier);
        await Task.WhenAll(
            _refreshTokenService.RevokeModelTokensAsync(driver.Id, "Driver"),
            _jwtTokenService.RevokeAllUserTokensAsync(driver.Id, "driver", expirationMinutes: 120));
        return true;
    }

    private async Task<bool> HandleNullResetAttemptAsync(string identifier)
    {
        await _passwordResetCacheService.IncrementFailedAttemptsAsync("driver", identifier);
        throw new UnauthorizedException("auth.invalid_or_expired_reset_code");
    }

    private async Task<bool> HandleInvalidResetCodeAsync(string identifier, Domain.Common.Interfaces.PasswordResetAttempt resetAttempt)
    {
        await _passwordResetCacheService.IncrementFailedAttemptsAsync("driver", identifier);
        await (resetAttempt.FailedAttempts >= 4
            ? _passwordResetCacheService.DeleteResetAttemptAsync("driver", identifier)
            : Task.CompletedTask);
        throw new UnauthorizedException("auth.invalid_or_expired_reset_code");
    }

    private async Task<bool> HandleDriverNotFoundAsync(string identifier)
    {
        await _passwordResetCacheService.DeleteResetAttemptAsync("driver", identifier);
        throw new NotFoundException("auth.user_not_found");
    }
}

