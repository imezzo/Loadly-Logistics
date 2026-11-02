using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Domain.Driver.DTOs.Auth;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Driver.Actions.Auth;






public class DriverResetPasswordAction : IAction<DriverResetPasswordDto, bool>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly IPasswordResetTokenService _passwordResetTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;
    public DriverResetPasswordAction(
        IDriverRepository driverRepository,
        IPasswordHashingService passwordHashingService,
        IPasswordResetTokenService passwordResetTokenService,
        IRefreshTokenService refreshTokenService,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _driverRepository = driverRepository;
        _passwordHashingService = passwordHashingService;
        _passwordResetTokenService = passwordResetTokenService;
        _refreshTokenService = refreshTokenService;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }
    public async Task<bool> ExecuteAsync(DriverResetPasswordDto dto)
    {
        ValidatePasswordsMatch(dto);
        ValidatePasswordStrength(dto.NewPassword);
        var tokenData = _passwordResetTokenService.ValidateResetToken(dto.Token)
            ?? throw new UnauthorizedException("auth.invalid_or_expired_reset_token");
        _ = tokenData.UserType == "driver"
            ? true
            : throw new UnauthorizedException("auth.invalid_token_for_driver");
        var driver = await _driverRepository.GetByIdAsync(tokenData.UserId);
        UserValidator.ValidateUserExists(driver, "Driver");
        UpdatePassword(driver!, dto.NewPassword);
        VerifyChannelBasedOnToken(driver!, tokenData.Channel);
        await _driverRepository.UpdateAsync(driver!);
        await _unitOfWork.SaveChangesAsync();
        await Task.WhenAll(
            _refreshTokenService.RevokeModelTokensAsync(driver!.Id, "Driver"),
            _jwtTokenService.RevokeAllUserTokensAsync(driver!.Id, "driver", expirationMinutes: 120));
        return true;
    }
    private static void ValidatePasswordsMatch(DriverResetPasswordDto dto)
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
    private void UpdatePassword(Domain.Common.Entities.Driver driver, string newPassword)
    {
        driver.Password = _passwordHashingService.HashPassword(newPassword);
    }
    private void VerifyChannelBasedOnToken(Domain.Common.Entities.Driver driver, CommunicationChannel channel)
    {
        driver.EmailVerified = channel == CommunicationChannel.Email ? true : driver.EmailVerified;
        driver.PhoneVerified = channel == CommunicationChannel.Sms ? true : driver.PhoneVerified;
    }
}
