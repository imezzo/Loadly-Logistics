using LoadlyAggregator.Domain.Collector.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;
namespace LoadlyAggregator.Domain.Collector.Actions.Auth;



public class CollectorChangePasswordAction : IAction<CollectorChangePasswordDto, bool>
{
    private readonly ICollectorRepository _collectorRepository;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtTokenService _jwtTokenService;
    public CollectorChangePasswordAction(
        ICollectorRepository collectorRepository,
        IPasswordHashingService passwordHashingService,
        IRefreshTokenService refreshTokenService,
        IJwtTokenService jwtTokenService)
    {
        _collectorRepository = collectorRepository;
        _passwordHashingService = passwordHashingService;
        _refreshTokenService = refreshTokenService;
        _jwtTokenService = jwtTokenService;
    }
    public async Task<bool> ExecuteAsync(CollectorChangePasswordDto dto)
    {
        var collector = await _collectorRepository.GetByIdAsync(dto.UserId)
            ?? throw new NotFoundException("collectors.not_found");
        _ = _passwordHashingService.VerifyPassword(dto.CurrentPassword, collector.Password)
            ? true
            : throw new ValidationException("auth.change_password_current_incorrect");
        collector.Password = _passwordHashingService.HashPassword(dto.NewPassword);
        await _collectorRepository.UpdateAsync(collector);
        await Task.WhenAll(
            _refreshTokenService.RevokeModelTokensAsync(collector.Id, "Collector"),
            _jwtTokenService.RevokeAllUserTokensAsync(collector.Id, "collector", expirationMinutes: 120));
        return true;
    }
}
