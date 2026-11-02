using LoadlyAggregator.Domain.Admin.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;
namespace LoadlyAggregator.Domain.Admin.Actions.Auth;






public class AdminResetPasswordAction : IAction<AdminResetPasswordDto, bool>
{
    private readonly IAdminRepository _adminRepository;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly IPasswordResetTokenService _passwordResetTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;
    public AdminResetPasswordAction(
        IAdminRepository adminRepository,
        IPasswordHashingService passwordHashingService,
        IPasswordResetTokenService passwordResetTokenService,
        IRefreshTokenService refreshTokenService,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _adminRepository = adminRepository;
        _passwordHashingService = passwordHashingService;
        _passwordResetTokenService = passwordResetTokenService;
        _refreshTokenService = refreshTokenService;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }
    public async Task<bool> ExecuteAsync(AdminResetPasswordDto dto)
    {
        _ = dto.NewPassword == dto.ConfirmPassword
            ? true
            : throw new ValidationException("auth.password_mismatch");
        var tokenData = _passwordResetTokenService.ValidateResetToken(dto.Token)
            ?? throw new UnauthorizedException("auth.invalid_or_expired_reset_token");
        _ = tokenData.UserType == "admin"
            ? true
            : throw new UnauthorizedException("auth.invalid_token_for_driver");
        var admin = await _adminRepository.GetByIdAsync(tokenData.UserId)
            ?? throw new NotFoundException("auth.user_not_found");
        _ = !string.IsNullOrWhiteSpace(dto.NewPassword) && dto.NewPassword.Length >= 6
            ? true
            : throw new ValidationException("auth.password_min_length");
        admin.Password = _passwordHashingService.HashPassword(dto.NewPassword);
        admin.EmailVerified = tokenData.Channel == CommunicationChannel.Email ? true : admin.EmailVerified;
        admin.PhoneVerified = tokenData.Channel == CommunicationChannel.Sms ? true : admin.PhoneVerified;
        await _adminRepository.UpdateAsync(admin);
        await _unitOfWork.SaveChangesAsync();
        await Task.WhenAll(
            _refreshTokenService.RevokeModelTokensAsync(admin.Id, "Admin"),
            _jwtTokenService.RevokeAllUserTokensAsync(admin.Id, "admin", expirationMinutes: 120));
        return true;
    }
}
