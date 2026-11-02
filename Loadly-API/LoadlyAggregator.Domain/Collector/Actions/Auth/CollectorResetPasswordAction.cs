using LoadlyAggregator.Domain.Collector.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;
namespace LoadlyAggregator.Domain.Collector.Actions.Auth;






public class CollectorResetPasswordAction : IAction<CollectorResetPasswordDto, bool>
{
    private readonly ICollectorRepository _collectorRepository;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly IPasswordResetTokenService _passwordResetTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;
    public CollectorResetPasswordAction(
        ICollectorRepository collectorRepository,
        IPasswordHashingService passwordHashingService,
        IPasswordResetTokenService passwordResetTokenService,
        IRefreshTokenService refreshTokenService,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _collectorRepository = collectorRepository;
        _passwordHashingService = passwordHashingService;
        _passwordResetTokenService = passwordResetTokenService;
        _refreshTokenService = refreshTokenService;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }
    public async Task<bool> ExecuteAsync(CollectorResetPasswordDto dto)
    {
        _ = dto.NewPassword == dto.ConfirmPassword
            ? true
            : throw new ValidationException("auth.password_mismatch");
        var tokenData = _passwordResetTokenService.ValidateResetToken(dto.Token)
            ?? throw new UnauthorizedException("auth.invalid_or_expired_reset_token");
        _ = tokenData.UserType == "collector"
            ? true
            : throw new UnauthorizedException("auth.invalid_token_for_driver");
        var collector = await _collectorRepository.GetByIdAsync(tokenData.UserId)
            ?? throw new NotFoundException("auth.user_not_found");
        _ = !string.IsNullOrWhiteSpace(dto.NewPassword) && dto.NewPassword.Length >= 6
            ? true
            : throw new ValidationException("auth.password_min_length");
        collector.Password = _passwordHashingService.HashPassword(dto.NewPassword);
        collector.EmailVerified = tokenData.Channel == CommunicationChannel.Email ? true : collector.EmailVerified;
        collector.PhoneVerified = tokenData.Channel == CommunicationChannel.Sms ? true : collector.PhoneVerified;
        await _collectorRepository.UpdateAsync(collector);
        await _unitOfWork.SaveChangesAsync();
        await Task.WhenAll(
            _refreshTokenService.RevokeModelTokensAsync(collector.Id, "Collector"),
            _jwtTokenService.RevokeAllUserTokensAsync(collector.Id, "collector", expirationMinutes: 120));
        return true;
    }
}
