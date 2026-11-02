using LoadlyAggregator.Domain.Admin.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Auth;






public class AdminResetPasswordWithCodeAction : IAction<AdminResetPasswordWithCodeDto, bool>
{
    private readonly IAdminRepository _adminRepository;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly IPasswordResetCacheService _passwordResetCacheService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public AdminResetPasswordWithCodeAction(
        IAdminRepository adminRepository,
        IPasswordHashingService passwordHashingService,
        IPasswordResetCacheService passwordResetCacheService,
        IRefreshTokenService refreshTokenService,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _adminRepository = adminRepository;
        _passwordHashingService = passwordHashingService;
        _passwordResetCacheService = passwordResetCacheService;
        _refreshTokenService = refreshTokenService;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> ExecuteAsync(AdminResetPasswordWithCodeDto dto)
    {
        _ = dto.NewPassword == dto.ConfirmPassword
            ? true
            : throw new ValidationException("auth.password_mismatch");
        _ = await _passwordResetCacheService.IsRateLimitedAsync("admin", dto.Identifier)
            ? throw new TooManyRequestsException("auth.too_many_failed_attempts")
            : true;
        var resetAttempt = await _passwordResetCacheService.GetResetAttemptAsync("admin", dto.Identifier);
        _ = resetAttempt != null
            ? true
            : await HandleNullResetAttemptAsync(dto.Identifier);
        _ = resetAttempt!.Code.Equals(dto.Code, StringComparison.OrdinalIgnoreCase)
            ? true
            : await HandleInvalidResetCodeAsync(dto.Identifier, resetAttempt);
        var admin = await _adminRepository.GetByIdAsync(resetAttempt.UserId);
        _ = admin != null
            ? true
            : await HandleAdminNotFoundAsync(dto.Identifier);
        _ = !string.IsNullOrWhiteSpace(dto.NewPassword) && dto.NewPassword.Length >= 6
            ? true
            : throw new ValidationException("auth.password_min_length");
        admin!.Password = _passwordHashingService.HashPassword(dto.NewPassword);
        admin.EmailVerified = resetAttempt.Channel == CommunicationChannel.Email ? true : admin.EmailVerified;
        admin.PhoneVerified = resetAttempt.Channel == CommunicationChannel.Sms ? true : admin.PhoneVerified;
        await _adminRepository.UpdateAsync(admin);
        await _unitOfWork.SaveChangesAsync();
        await _passwordResetCacheService.DeleteResetAttemptAsync("admin", dto.Identifier);
        await Task.WhenAll(
            _refreshTokenService.RevokeModelTokensAsync(admin.Id, "Admin"),
            _jwtTokenService.RevokeAllUserTokensAsync(admin.Id, "admin", expirationMinutes: 120));
        return true;
    }

    private async Task<bool> HandleNullResetAttemptAsync(string identifier)
    {
        await _passwordResetCacheService.IncrementFailedAttemptsAsync("admin", identifier);
        throw new UnauthorizedException("auth.invalid_or_expired_reset_code");
    }

    private async Task<bool> HandleInvalidResetCodeAsync(string identifier, Domain.Common.Interfaces.PasswordResetAttempt resetAttempt)
    {
        await _passwordResetCacheService.IncrementFailedAttemptsAsync("admin", identifier);
        await (resetAttempt.FailedAttempts >= 4
            ? _passwordResetCacheService.DeleteResetAttemptAsync("admin", identifier)
            : Task.CompletedTask);
        throw new UnauthorizedException("auth.invalid_or_expired_reset_code");
    }

    private async Task<bool> HandleAdminNotFoundAsync(string identifier)
    {
        await _passwordResetCacheService.DeleteResetAttemptAsync("admin", identifier);
        throw new NotFoundException("auth.user_not_found");
    }
}

