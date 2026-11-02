using LoadlyAggregator.Domain.Collector.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Exceptions;
using MfaCodeAttempt = LoadlyAggregator.Domain.Common.Interfaces.MfaCodeAttempt;

namespace LoadlyAggregator.Domain.Collector.Actions.Auth;





public class CollectorVerifyPhoneAction : IAction<CollectorVerifyPhoneDto, object>
{
    private readonly ICollectorRepository _collectorRepository;
    private readonly IMfaCacheService _mfaCacheService;

    public CollectorVerifyPhoneAction(
        ICollectorRepository collectorRepository,
        IMfaCacheService mfaCacheService)
    {
        _collectorRepository = collectorRepository;
        _mfaCacheService = mfaCacheService;
    }

    public async Task<object> ExecuteAsync(CollectorVerifyPhoneDto dto)
    {
        ValidateRequest(dto);
        var collector = await _collectorRepository.GetByIdAsync(dto.UserId);
        UserValidator.ValidateUserExists(collector, "Collector");
        _ = collector!.PhoneVerified
            ? throw new ValidationException("validation.phone_already_verified")
            : true;
        _ = await _mfaCacheService.IsRateLimitedAsync(dto.UserId, "collector")
            ? throw new TooManyRequestsException("auth.too_many_failed_attempts")
            : true;
        var codeAttempt = await _mfaCacheService.GetMfaCodeAsync(dto.UserId, "collector");
        _ = codeAttempt != null
            ? true
            : await HandleNullCodeAttemptAsync(dto.UserId);
        _ = codeAttempt!.Channel == CommunicationChannel.Sms
            ? true
            : await HandleWrongChannelAsync(dto.UserId);
        _ = codeAttempt.Code.Equals(dto.Code, StringComparison.OrdinalIgnoreCase)
            ? true
            : await HandleInvalidCodeAsync(dto.UserId, codeAttempt);
        collector.PhoneVerified = true;
        await _collectorRepository.UpdateAsync(collector);
        await _mfaCacheService.DeleteMfaCodeAsync(dto.UserId, "collector");
        return new { };
    }

    private async Task<bool> HandleNullCodeAttemptAsync(long userId)
    {
        await _mfaCacheService.IncrementFailedAttemptsAsync(userId, "collector");
        throw new ValidationException("auth.invalid_or_expired_verification_code");
    }

    private async Task<bool> HandleWrongChannelAsync(long userId)
    {
        await _mfaCacheService.IncrementFailedAttemptsAsync(userId, "collector");
        throw new ValidationException("auth.code_for_email_not_phone");
    }

    private async Task<bool> HandleInvalidCodeAsync(long userId, MfaCodeAttempt codeAttempt)
    {
        await _mfaCacheService.IncrementFailedAttemptsAsync(userId, "collector");
        await (codeAttempt.FailedAttempts >= 4
            ? _mfaCacheService.DeleteMfaCodeAsync(userId, "collector")
            : Task.CompletedTask);
        throw new ValidationException("auth.invalid_or_expired_verification_code");
    }

    private static void ValidateRequest(CollectorVerifyPhoneDto dto)
    {
        _ = string.IsNullOrWhiteSpace(dto.Code)
            ? throw new ValidationException("validation.verification_code_required")
            : true;
        _ = dto.UserId <= 0
            ? throw new ValidationException("validation.user_id_required")
            : true;
    }
}

