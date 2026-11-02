using LoadlyAggregator.Domain.Admin.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;
namespace LoadlyAggregator.Domain.Admin.Actions.Auth;



public class AdminChangePasswordAction : IAction<AdminChangePasswordDto, bool>
{
    private readonly IAdminRepository _adminRepository;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtTokenService _jwtTokenService;
    public AdminChangePasswordAction(
        IAdminRepository adminRepository,
        IPasswordHashingService passwordHashingService,
        IRefreshTokenService refreshTokenService,
        IJwtTokenService jwtTokenService)
    {
        _adminRepository = adminRepository;
        _passwordHashingService = passwordHashingService;
        _refreshTokenService = refreshTokenService;
        _jwtTokenService = jwtTokenService;
    }
    public async Task<bool> ExecuteAsync(AdminChangePasswordDto dto)
    {
        var admin = await _adminRepository.GetByIdAsync(dto.UserId)
            ?? throw new NotFoundException("admins.not_found");
        _ = _passwordHashingService.VerifyPassword(dto.CurrentPassword, admin.Password)
            ? true
            : throw new ValidationException("auth.change_password_current_incorrect");
        admin.Password = _passwordHashingService.HashPassword(dto.NewPassword);
        await _adminRepository.UpdateAsync(admin);
        await Task.WhenAll(
            _refreshTokenService.RevokeModelTokensAsync(admin.Id, "Admin"),
            _jwtTokenService.RevokeAllUserTokensAsync(admin.Id, "admin", expirationMinutes: 120));
        return true;
    }
}
