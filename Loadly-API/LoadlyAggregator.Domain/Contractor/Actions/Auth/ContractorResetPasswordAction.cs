using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Domain.Contractor.DTOs.Auth;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Contractor.Actions.Auth;


public class ContractorResetPasswordAction : IAction<ContractorResetPasswordDto, bool>
{
    private readonly IContractorRepository _contractorRepository;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly IPasswordResetTokenService _passwordResetTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;
    public ContractorResetPasswordAction(
        IContractorRepository contractorRepository,
        IPasswordHashingService passwordHashingService,
        IPasswordResetTokenService passwordResetTokenService,
        IRefreshTokenService refreshTokenService,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _contractorRepository = contractorRepository;
        _passwordHashingService = passwordHashingService;
        _passwordResetTokenService = passwordResetTokenService;
        _refreshTokenService = refreshTokenService;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }
    public async Task<bool> ExecuteAsync(ContractorResetPasswordDto dto)
    {
        ValidatePasswordsMatch(dto);
        ValidatePasswordStrength(dto.NewPassword);
        var tokenData = _passwordResetTokenService.ValidateResetToken(dto.Token)
            ?? throw new UnauthorizedException("auth.invalid_or_expired_reset_token");
        _ = tokenData.UserType == "contractor"
            ? true
            : throw new UnauthorizedException("auth.invalid_token_for_Contractor");
        var contractor = await _contractorRepository.GetByIdAsync(tokenData.UserId);
        UserValidator.ValidateUserExists(contractor, "Contractor");
        UpdatePassword(contractor!, dto.NewPassword);
        VerifyChannelBasedOnToken(contractor!, tokenData.Channel);
        await _contractorRepository.UpdateAsync(contractor!);
        await _unitOfWork.SaveChangesAsync();
        await Task.WhenAll(
            _refreshTokenService.RevokeModelTokensAsync(contractor!.Id, "Contractor"),
            _jwtTokenService.RevokeAllUserTokensAsync(contractor!.Id, "contractor", expirationMinutes: 120));
        return true;
    }
    private static void ValidatePasswordsMatch(ContractorResetPasswordDto dto)
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
    private void UpdatePassword(Domain.Common.Entities.Contractor contractor, string newPassword)
    {
        contractor.Password = _passwordHashingService.HashPassword(newPassword);
    }
    private void VerifyChannelBasedOnToken(Domain.Common.Entities.Contractor contractor, CommunicationChannel channel)
    {
        contractor.EmailVerified = channel == CommunicationChannel.Email ? true : contractor.EmailVerified;
        contractor.PhoneVerified = channel == CommunicationChannel.Sms ? true : contractor.PhoneVerified;
    }
}
