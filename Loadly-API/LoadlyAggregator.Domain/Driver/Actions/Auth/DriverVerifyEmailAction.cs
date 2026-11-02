using LoadlyAggregator.Domain.Common.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Domain.Driver.DTOs.Auth;
using LoadlyAggregator.Shared.Exceptions;
using MfaCodeAttempt = LoadlyAggregator.Domain.Common.Interfaces.MfaCodeAttempt;

namespace LoadlyAggregator.Domain.Driver.Actions.Auth;





public class DriverVerifyEmailAction : IAction<DriverVerifyEmailDto, object>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IMfaCacheService _mfaCacheService;

    public DriverVerifyEmailAction(
        IDriverRepository driverRepository,
        IMfaCacheService mfaCacheService)
    {
        _driverRepository = driverRepository;
        _mfaCacheService = mfaCacheService;
    }

    public async Task<object> ExecuteAsync(DriverVerifyEmailDto dto)
    {
        ValidateRequest(dto);
        var driver = await _driverRepository.GetByIdAsync(dto.UserId);
        UserValidator.ValidateUserExists(driver, "Driver");
        _ = !string.IsNullOrWhiteSpace(driver!.Email)
            ? true
            : throw new ValidationException("auth.no_email_to_verify");
        _ = driver.EmailVerified
            ? throw new ValidationException("validation.email_already_verified")
            : true;
        _ = await _mfaCacheService.IsRateLimitedAsync(dto.UserId, "driver")
            ? throw new TooManyRequestsException("auth.too_many_failed_attempts")
            : true;
        var codeAttempt = await _mfaCacheService.GetMfaCodeAsync(dto.UserId, "driver");
        _ = codeAttempt != null
            ? true
            : await HandleNullCodeAttemptAsync(dto.UserId);
        _ = codeAttempt!.Channel == CommunicationChannel.Email
            ? true
            : await HandleWrongChannelAsync(dto.UserId);
        _ = codeAttempt.Code.Equals(dto.Code, StringComparison.OrdinalIgnoreCase)
            ? true
            : await HandleInvalidCodeAsync(dto.UserId, codeAttempt);
        driver.EmailVerified = true;
        await _driverRepository.UpdateAsync(driver);
        await _driverRepository.SaveChangesAsync();
        await _mfaCacheService.DeleteMfaCodeAsync(dto.UserId, "driver");
        return new { };
    }

    private async Task<bool> HandleNullCodeAttemptAsync(long userId)
    {
        await _mfaCacheService.IncrementFailedAttemptsAsync(userId, "driver");
        throw new ValidationException("auth.invalid_or_expired_verification_code");
    }

    private async Task<bool> HandleWrongChannelAsync(long userId)
    {
        await _mfaCacheService.IncrementFailedAttemptsAsync(userId, "driver");
        throw new ValidationException("auth.code_for_phone_not_email");
    }

    private async Task<bool> HandleInvalidCodeAsync(long userId, MfaCodeAttempt codeAttempt)
    {
        await _mfaCacheService.IncrementFailedAttemptsAsync(userId, "driver");
        await (codeAttempt.FailedAttempts >= 4
            ? _mfaCacheService.DeleteMfaCodeAsync(userId, "driver")
            : Task.CompletedTask);
        throw new ValidationException("auth.invalid_or_expired_verification_code");
    }

    private static void ValidateRequest(DriverVerifyEmailDto dto)
    {
        _ = string.IsNullOrWhiteSpace(dto.Code)
            ? throw new ValidationException("validation.verification_code_required")
            : true;
        _ = dto.UserId <= 0
            ? throw new ValidationException("validation.user_id_required")
            : true;
    }
}

