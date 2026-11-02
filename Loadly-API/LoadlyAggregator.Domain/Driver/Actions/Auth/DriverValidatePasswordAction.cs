using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Auth;
using LoadlyAggregator.Domain.Driver.DTOs.Auth;

namespace LoadlyAggregator.Domain.Driver.Actions.Auth;

public class DriverValidatePasswordAction : IVoidAction<DriverValidatePasswordDto>
{
    private readonly IDriverRepository _driverRepository;
    private readonly AuthService _authService;

    public DriverValidatePasswordAction(
        IDriverRepository driverRepository,
        AuthService authService)
    {
        _driverRepository = driverRepository;
        _authService = authService;
    }

    public async Task ExecuteAsync(DriverValidatePasswordDto dto)
    {
        _authService.ValidateCredentials(
            await _driverRepository.GetByIdAsync(dto.UserId),
            dto.Password);
    }
}



