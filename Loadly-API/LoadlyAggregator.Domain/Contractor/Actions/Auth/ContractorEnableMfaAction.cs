using LoadlyAggregator.Domain.Common.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Domain.Contractor.DTOs.Auth;

namespace LoadlyAggregator.Domain.Contractor.Actions.Auth;




public class ContractorEnableMfaAction : IAction<ContractorEnableMfaDto, bool>
{
    private readonly IContractorRepository _contractorRepository;

    public ContractorEnableMfaAction(IContractorRepository contractorRepository)
    {
        _contractorRepository = contractorRepository;
    }

    public async Task<bool> ExecuteAsync(ContractorEnableMfaDto dto)
    {
        UserValidator.ValidateMfaChannel(dto.Channel);

        var contractor = await _contractorRepository.GetByIdAsync(dto.UserId);
        UserValidator.ValidateUserExists(contractor, "Contractor");


        UserValidator.ValidateChannelVerified(dto.Channel, contractor!.EmailVerified, contractor.PhoneVerified);

        EnableMfa(contractor!, dto.Channel);
        await _contractorRepository.UpdateAsync(contractor!);

        return true;
    }

    private static void EnableMfa(Domain.Common.Entities.Contractor contractor, CommunicationChannel channel)
    {

        contractor.MfaChannel = channel;
    }
}

