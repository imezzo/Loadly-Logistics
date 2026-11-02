using LoadlyAggregator.Domain.Common.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Domain.Driver.DTOs.Auth;
using LoadlyAggregator.Shared.Exceptions;
namespace LoadlyAggregator.Domain.Driver.Actions.Auth;



public class DriverChangePasswordAction : IAction<DriverChangePasswordDto, bool>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtTokenService _jwtTokenService;
    public DriverChangePasswordAction(
        IDriverRepository driverRepository,
        IPasswordHashingService passwordHashingService,
        IRefreshTokenService refreshTokenService,
        IJwtTokenService jwtTokenService)
    {
        _driverRepository = driverRepository;
        _passwordHashingService = passwordHashingService;
        _refreshTokenService = refreshTokenService;
        _jwtTokenService = jwtTokenService;
    }
    public async Task<bool> ExecuteAsync(DriverChangePasswordDto dto)
    {
        var driver = await _driverRepository.GetByIdAsync(dto.UserId);
        UserValidator.ValidateUserExists(driver, "Driver");
        UserValidator.ValidatePassword(_passwordHashingService, dto.CurrentPassword, driver!.Password);
        UpdatePassword(driver, dto.NewPassword);
        await _driverRepository.UpdateAsync(driver);
        await Task.WhenAll(
            _refreshTokenService.RevokeModelTokensAsync(driver.Id, "Driver"),
            _jwtTokenService.RevokeAllUserTokensAsync(driver.Id, "driver", expirationMinutes: 120));
        return true;
    }
    private void UpdatePassword(Domain.Common.Entities.Driver driver, string newPassword)
    {
        driver.Password = _passwordHashingService.HashPassword(newPassword);
    }
}
