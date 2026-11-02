using LoadlyAggregator.Domain.Common.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Domain.Company.DTOs.Auth;
using LoadlyAggregator.Shared.Exceptions;
using MfaCodeAttempt = LoadlyAggregator.Domain.Common.Interfaces.MfaCodeAttempt;

namespace LoadlyAggregator.Domain.Company.Actions.Auth;





public class CompanyVerifyEmailAction : IAction<CompanyVerifyEmailDto, object>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IMfaCacheService _mfaCacheService;

    public CompanyVerifyEmailAction(
        ICompanyRepository companyRepository,
        IMfaCacheService mfaCacheService)
    {
        _companyRepository = companyRepository;
        _mfaCacheService = mfaCacheService;
    }

    public async Task<object> ExecuteAsync(CompanyVerifyEmailDto dto)
    {
        ValidateRequest(dto);
        var company = await _companyRepository.GetByIdAsync(dto.UserId);
        UserValidator.ValidateUserExists(company, "Company");
        _ = company!.EmailVerified
            ? throw new ValidationException("validation.email_already_verified")
            : true;
        _ = await _mfaCacheService.IsRateLimitedAsync(dto.UserId, "company")
            ? throw new TooManyRequestsException("auth.too_many_failed_attempts")
            : true;
        var codeAttempt = await _mfaCacheService.GetMfaCodeAsync(dto.UserId, "company");
        _ = codeAttempt != null
            ? true
            : await HandleNullCodeAttemptAsync(dto.UserId);
        _ = codeAttempt!.Channel == CommunicationChannel.Email
            ? true
            : await HandleWrongChannelAsync(dto.UserId);
        _ = codeAttempt.Code.Equals(dto.Code, StringComparison.OrdinalIgnoreCase)
            ? true
            : await HandleInvalidCodeAsync(dto.UserId, codeAttempt);
        company.EmailVerified = true;
        await _companyRepository.UpdateAsync(company);
        await _mfaCacheService.DeleteMfaCodeAsync(dto.UserId, "company");
        return new { };
    }

    private async Task<bool> HandleNullCodeAttemptAsync(long userId)
    {
        await _mfaCacheService.IncrementFailedAttemptsAsync(userId, "company");
        throw new ValidationException("auth.invalid_or_expired_verification_code");
    }

    private async Task<bool> HandleWrongChannelAsync(long userId)
    {
        await _mfaCacheService.IncrementFailedAttemptsAsync(userId, "company");
        throw new ValidationException("auth.code_for_phone_not_email");
    }

    private async Task<bool> HandleInvalidCodeAsync(long userId, MfaCodeAttempt codeAttempt)
    {
        await _mfaCacheService.IncrementFailedAttemptsAsync(userId, "company");
        await (codeAttempt.FailedAttempts >= 4
            ? _mfaCacheService.DeleteMfaCodeAsync(userId, "company")
            : Task.CompletedTask);
        throw new ValidationException("auth.invalid_or_expired_verification_code");
    }

    private static void ValidateRequest(CompanyVerifyEmailDto dto)
    {
        _ = string.IsNullOrWhiteSpace(dto.Code)
            ? throw new ValidationException("validation.verification_code_required")
            : true;
        _ = dto.UserId <= 0
            ? throw new ValidationException("validation.user_id_required")
            : true;
    }
}

