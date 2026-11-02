using LoadlyAggregator.Domain.Common.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Domain.Contractor.DTOs.Auth;
using LoadlyAggregator.Shared.Exceptions;
using MfaCodeAttempt = LoadlyAggregator.Domain.Common.Interfaces.MfaCodeAttempt;

namespace LoadlyAggregator.Domain.Contractor.Actions.Auth;





public class ContractorVerifyPhoneAction : IAction<ContractorVerifyPhoneDto, object>
{
    private readonly IContractorRepository _contractorRepository;
    private readonly IMfaCacheService _mfaCacheService;

    public ContractorVerifyPhoneAction(
        IContractorRepository contractorRepository,
        IMfaCacheService mfaCacheService)
    {
        _contractorRepository = contractorRepository;
        _mfaCacheService = mfaCacheService;
    }

    public async Task<object> ExecuteAsync(ContractorVerifyPhoneDto dto)
    {
        ValidateRequest(dto);
        var contractor = await _contractorRepository.GetByIdAsync(dto.UserId);
        UserValidator.ValidateUserExists(contractor, "Contractor");
        _ = contractor!.PhoneVerified
            ? throw new ValidationException("validation.phone_already_verified")
            : true;
        _ = await _mfaCacheService.IsRateLimitedAsync(dto.UserId, "contractor")
            ? throw new TooManyRequestsException("auth.too_many_failed_attempts")
            : true;
        var codeAttempt = await _mfaCacheService.GetMfaCodeAsync(dto.UserId, "contractor");
        _ = codeAttempt != null
            ? true
            : await HandleNullCodeAttemptAsync(dto.UserId);
        _ = codeAttempt!.Channel == CommunicationChannel.Sms
            ? true
            : await HandleWrongChannelAsync(dto.UserId);
        _ = codeAttempt.Code.Equals(dto.Code, StringComparison.OrdinalIgnoreCase)
            ? true
            : await HandleInvalidCodeAsync(dto.UserId, codeAttempt);
        contractor.PhoneVerified = true;
        await _contractorRepository.UpdateAsync(contractor);
        await _mfaCacheService.DeleteMfaCodeAsync(dto.UserId, "contractor");
        return new { };
    }

    private async Task<bool> HandleNullCodeAttemptAsync(long userId)
    {
        await _mfaCacheService.IncrementFailedAttemptsAsync(userId, "contractor");
        throw new ValidationException("auth.invalid_or_expired_verification_code");
    }

    private async Task<bool> HandleWrongChannelAsync(long userId)
    {
        await _mfaCacheService.IncrementFailedAttemptsAsync(userId, "contractor");
        throw new ValidationException("auth.code_for_email_not_phone");
    }

    private async Task<bool> HandleInvalidCodeAsync(long userId, MfaCodeAttempt codeAttempt)
    {
        await _mfaCacheService.IncrementFailedAttemptsAsync(userId, "contractor");
        await (codeAttempt.FailedAttempts >= 4
            ? _mfaCacheService.DeleteMfaCodeAsync(userId, "contractor")
            : Task.CompletedTask);
        throw new ValidationException("auth.invalid_or_expired_verification_code");
    }

    private static void ValidateRequest(ContractorVerifyPhoneDto dto)
    {
        _ = string.IsNullOrWhiteSpace(dto.Code)
            ? throw new ValidationException("validation.verification_code_required")
            : true;
        _ = dto.UserId <= 0
            ? throw new ValidationException("validation.user_id_required")
            : true;
    }
}

