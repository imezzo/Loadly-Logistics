using LoadlyAggregator.Domain.Common.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Domain.Driver.DTOs.Auth;
namespace LoadlyAggregator.Domain.Driver.Actions.Auth;



public class DriverDisableMfaAction : IAction<DriverDisableMfaDto, bool>
{
    private readonly IDriverRepository _driverRepository;
    public DriverDisableMfaAction(IDriverRepository driverRepository)
    {
        _driverRepository = driverRepository;
    }
    public async Task<bool> ExecuteAsync(DriverDisableMfaDto dto)
    {
        var driver = await _driverRepository.GetByIdAsync(dto.UserId);
        UserValidator.ValidateUserExists(driver, "Driver");

        driver!.MfaChannel = CommunicationChannel.None;
        await _driverRepository.UpdateAsync(driver);
        return true;
    }
}
