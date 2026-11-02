using LoadlyAggregator.Domain.Common.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Domain.Driver.DTOs.Auth;

namespace LoadlyAggregator.Domain.Driver.Actions.Auth;




public class DriverEnableMfaAction : IAction<DriverEnableMfaDto, bool>
{
    private readonly IDriverRepository _driverRepository;

    public DriverEnableMfaAction(IDriverRepository driverRepository)
    {
        _driverRepository = driverRepository;
    }

    public async Task<bool> ExecuteAsync(DriverEnableMfaDto dto)
    {
        UserValidator.ValidateMfaChannel(dto.Channel);

        var driver = await _driverRepository.GetByIdAsync(dto.UserId);
        UserValidator.ValidateUserExists(driver, "Driver");


        UserValidator.ValidateChannelVerified(dto.Channel, driver!.EmailVerified, driver.PhoneVerified);

        EnableMfa(driver!, dto.Channel);
        await _driverRepository.UpdateAsync(driver!);

        return true;
    }

    private static void EnableMfa(Domain.Common.Entities.Driver driver, CommunicationChannel channel)
    {

        driver.MfaChannel = channel;
    }
}

